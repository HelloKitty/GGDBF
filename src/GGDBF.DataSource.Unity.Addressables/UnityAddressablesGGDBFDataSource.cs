using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GGDBF
{
	/// <summary>
	/// Unity3D Addressables-based implementation of <see cref="IGGDBFDataSource"/>.
	/// </summary>
	public sealed class UnityAddressablesGGDBFDataSource : IGGDBFDataSource
	{
		private IGGDBFSerializer Serializer { get; }

		private string BasePath { get; }

		/// <summary>
		/// Indicates if the asset path for loading the GGDBF file should be lowercase.
		/// </summary>
		public bool ForceLowercasePath { get; }

		public UnityAddressablesGGDBFDataSource(IGGDBFSerializer serializer, string basePath, bool forceLowercasePath = false)
		{
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			BasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
			ForceLowercasePath = forceLowercasePath;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<TModelType>> RetrieveAllAsync<TModelType>(CancellationToken token = default) 
			where TModelType : class
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class
		{
			// Create the addressable GGDBF path
			// Unity requirement that the GGDBF file type is removed.
			// "A special feature of the text asset is that it can be used to store binary data.
			// By giving a file the extension .bytes it can be loaded as a text asset and the data can be accessed through the bytes property."
			var path = $"{BasePath}/{config.TableNameOverride}.bytes";

			if (ForceLowercasePath)
				path = path.ToLowerInvariant();

			// TODO: Do we need to do this?
			// Then begin loading it
			if (Application.platform != RuntimePlatform.WebGLPlayer
				&& UnityAsyncHelper.UnityMainThreadContext != SynchronizationContext.Current)
				await new UnityYieldAwaitable();

			// See: https://thegamedev.guru/unity-addressables/textasset-loading/
			AsyncOperationHandle<TextAsset> loadHandle = Addressables.LoadAssetAsync<TextAsset>(path);

			if (Application.platform != RuntimePlatform.WebGLPlayer)
				return await CreateNonDeferredTable<TPrimaryKeyType, TModelType>(token, loadHandle);
			else
				return await CreateDeferredTable<TPrimaryKeyType, TModelType>(loadHandle, config.TableNameOverride, token);
		}

		private async Task<GGDBFTable<TPrimaryKeyType, TModelType>> CreateDeferredTable<TPrimaryKeyType, TModelType>(AsyncOperationHandle<TextAsset> loadHandle, string tableName, CancellationToken token) where TModelType : class
		{
			// Still must wait till download/load, but can skip deserialization.
			await loadHandle.Task;

			return new GGDBFTable<TPrimaryKeyType, TModelType>(tableName, new DeferredDeserializedGGDBFTableDictionary<TPrimaryKeyType, TModelType>(loadHandle, Serializer));
		}

		private async Task<GGDBFTable<TPrimaryKeyType, TModelType>> CreateNonDeferredTable<TPrimaryKeyType, TModelType>(CancellationToken token, AsyncOperationHandle<TextAsset> loadHandle) where TModelType : class
		{
			try
			{
				var ggdbfData = await loadHandle.Task;

				// Must be called on main thread still
				NativeArray<byte> underlyingDataBytes = ggdbfData.GetData<byte>();

				// There is no main thread, just 1 thread in WebGL so just deserialize.
				if (Application.platform == RuntimePlatform.WebGLPlayer)
				{
					// Unity in their infinite wisdom decided that calling bytes will cause a COPY
					// so let's directly access the underlying native bytes like it recommends
					// See: https://docs.unity3d.com/ScriptReference/TextAsset.GetData.html
					return Serializer.Deserialize<TPrimaryKeyType, TModelType>(underlyingDataBytes);
				}
				else
				{
					// This is so we don't block main game thread with deserializing.
					return await Task.Factory.StartNew(() => Deserialize<TPrimaryKeyType, TModelType>(underlyingDataBytes), token)
						.ConfigureAwait(false);
				}
			}
			finally
			{
				if (Application.platform != RuntimePlatform.WebGLPlayer
				    && UnityAsyncHelper.UnityMainThreadContext != SynchronizationContext.Current)
					await new UnityYieldAwaitable();

				Addressables.Release(loadHandle);
			}
		}

		private GGDBFTable<TPrimaryKeyType, TModelType> Deserialize<TPrimaryKeyType, TModelType>(NativeArray<byte> underlyingDataBytes) where TModelType : class
		{
			try
			{
				// Unity in their infinite wisdom decided that calling bytes will cause a COPY
				// so let's directly access the underlying native bytes like it recommends
				// See: https://docs.unity3d.com/ScriptReference/TextAsset.GetData.html
				return Serializer.Deserialize<TPrimaryKeyType, TModelType>(underlyingDataBytes);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to deserializer Type: {typeof(TModelType).FullName}. Reason: {e}", e);
			}
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			var table = await RetrieveFullTableAsync<TPrimaryKeyType, TSerializableModelType>(CastConfig<TPrimaryKeyType, TModelType, TSerializableModelType>(config), token);

			if (Application.platform != RuntimePlatform.WebGLPlayer)
				return table.ConvertFrom<TPrimaryKeyType, TModelType, TSerializableModelType>();
			else
				return new GGDBFTable<TPrimaryKeyType, TModelType>(table.TableName, new DeferredConvertableGGDBFTableDictionary<TPrimaryKeyType, TModelType, TSerializableModelType>(table.TableData));
		}

		/// <inheritdoc />
		public async Task ReloadAsync(CancellationToken token = default)
		{
			//There is nothing logically that can be done here.
		}

		private TableRetrievalConfig<TPrimaryKeyType, TSerializableModelType> CastConfig<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config)
			where TModelType : class
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			if (config == null)
				return null;

			Func<TSerializableModelType, TPrimaryKeyType> keyResolutionFuncCasted = (config.KeyResolutionFunction != null) ? m => config.KeyResolutionFunction(m) : null;
			return new TableRetrievalConfig<TPrimaryKeyType, TSerializableModelType>(keyResolutionFuncCasted, config?.TableNameOverride);
		}
	}
}
