using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	/// <summary>
	/// An HTTP <see cref="IGGDBFDataSource"/> implementation that uses File based loading and deserialization
	/// to source the data.
	/// </summary>
	public sealed class FileGGDBFDataSource<TGGDBFContextType> : IGGDBFDataSource
		where TGGDBFContextType : class, IGGDBFContext
	{
		/// <summary>
		/// The base URL for the requests.
		/// </summary>
		private string BasePath { get; }

		private IGGDBFSerializer SerializationStrategy { get; }

		public FileGGDBFDataSource(string basPath, IGGDBFSerializer serializationStrategy)
		{
			if (string.IsNullOrWhiteSpace(basPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(basPath));
			BasePath = basPath;
			SerializationStrategy = serializationStrategy ?? throw new ArgumentNullException(nameof(serializationStrategy));
		}

		/// <inheritdoc />
		public async Task<IEnumerable<TModelType>> RetrieveAllAsync<TModelType>(CancellationToken token = default) 
			where TModelType : class
		{
			// I think it's not possible
			throw new NotSupportedException($"TODO: Support unspecified key load from File");
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class
		{
			try
			{
				// netstandard2.0 doesn't have async read bytes from file sadly
				byte[] bytes = await ReadAllBytesAsync(Path.Combine(BasePath, $"{typeof(TModelType).Name}.ggdbf"), token);
				return SerializationStrategy.Deserialize<TPrimaryKeyType, TModelType>(bytes, 0, bytes.Length);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to load GGDBF Table Type: {typeof(TModelType).Name}. Reason: {e}", e);
			}
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			// I think it's not possible
			throw new NotSupportedException($"TODO: Support alternative serializable type load from File");
		}

		/// <inheritdoc />
		public Task ReloadAsync(CancellationToken token = default)
		{
			// Basically files don't need a reload, any loads will get latest from file directory.
			return Task.CompletedTask;
		}

		// netstandard2.0 doesn't have async read bytes from file sadly
		private static async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken token = default)
		{
			using FileStream sourceStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0, true);

			byte[] buffer = new byte[sourceStream.Length];

			using (MemoryStream ms = new MemoryStream(buffer, true))
				await sourceStream.CopyToAsync(ms, buffer.Length, token);

			return buffer;
		}
	}
}
