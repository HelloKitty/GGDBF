using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public sealed class FileGGDBFDataSource : IGGDBFDataSource
	{
		private IGGDBFSerializer Serializer { get; }

		private string OutputPath { get; }

		private bool WriteToCurrentDirectory => OutputPath == null;

		public FileGGDBFDataSource(IGGDBFSerializer serializer, string path = null)
		{
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			OutputPath = path;
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
			string path = WriteToCurrentDirectory
				? Path.Combine(Directory.GetCurrentDirectory(), $"{config.TableNameOverride}.{GGDBFConstants.FILE_EXTENSION_SUFFIX}")
				: Path.Combine(OutputPath, $"{config.TableNameOverride}.{GGDBFConstants.FILE_EXTENSION_SUFFIX}");

			using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

			var buffer = ArrayPool<byte>.Shared.Rent((int) fs.Length);
			try
			{
				await fs.ReadAsync(buffer, 0, (int)fs.Length, token);
				return Serializer.Deserialize<TPrimaryKeyType, TModelType>(buffer, 0, (int) fs.Length);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			var table = await RetrieveFullTableAsync<TPrimaryKeyType, TSerializableModelType>(CastConfig<TPrimaryKeyType, TModelType, TSerializableModelType>(config), token);

			return new GGDBFTable<TPrimaryKeyType, TModelType>()
			{
				TableName = table.TableName,
				Version = table.Version,
				TableData = table.TableData.ToDictionary(kvp => kvp.Key, kvp => (TModelType) kvp.Value)
			};
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
