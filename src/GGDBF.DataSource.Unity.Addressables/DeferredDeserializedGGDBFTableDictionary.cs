using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GGDBF
{
	/// <summary>
	/// Deferred deserialized <see cref="IReadOnlyDictionary{TKey,TValue}"/>
	/// for use in Unity3D WebGL to defer expensive loading operations.
	/// </summary>
	/// <typeparam name="TKeyType"></typeparam>
	/// <typeparam name="TModelType"></typeparam>
	public sealed class DeferredDeserializedGGDBFTableDictionary<TKeyType, TModelType>
		: IReadOnlyDictionary<TKeyType, TModelType>
	{
		private AsyncOperationHandle<TextAsset> LoadingHandle { get; set; }

		private IGGDBFSerializer Serializer { get; }

		private Lazy<IReadOnlyDictionary<TKeyType, TModelType>> _DeserializedDictionary { get; }
		private IReadOnlyDictionary<TKeyType, TModelType> DeserializedDictionary => _DeserializedDictionary.Value;

		public DeferredDeserializedGGDBFTableDictionary(AsyncOperationHandle<TextAsset> loadingHandle,
			[NotNull] IGGDBFSerializer serializer)
		{
			if (!loadingHandle.IsValid()) throw new ArgumentException("Value should be valid.", nameof(loadingHandle));
			LoadingHandle = loadingHandle;
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

			if (!LoadingHandle.IsDone)
				throw new InvalidOperationException($"The GGDBF loading handle must be downloaded before creating a deferred deserialized table.");

			_DeserializedDictionary = new Lazy<IReadOnlyDictionary<TKeyType, TModelType>>(() =>
			{
				try
				{
					// We're on WebGL for this so don't bother locking
					var asset = LoadingHandle.Result;
					var table = Serializer.Deserialize<TKeyType, TModelType>(asset.bytes);

					return table.TableData;
				}
				finally
				{
					Addressables.Release(LoadingHandle);
					LoadingHandle = default;
				}
			}, true);
		}

		public IEnumerator<KeyValuePair<TKeyType, TModelType>> GetEnumerator()
		{
			return DeserializedDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) DeserializedDictionary).GetEnumerator();
		}

		public int Count => DeserializedDictionary.Count;

		public bool ContainsKey(TKeyType key)
		{
			return DeserializedDictionary.ContainsKey(key);
		}

		public bool TryGetValue(TKeyType key, out TModelType value)
		{
			return DeserializedDictionary.TryGetValue(key, out value);
		}

		public TModelType this[TKeyType key] => DeserializedDictionary[key];

		public IEnumerable<TKeyType> Keys => DeserializedDictionary.Keys;

		public IEnumerable<TModelType> Values => DeserializedDictionary.Values;
	}
}
