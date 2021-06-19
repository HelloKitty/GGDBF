using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace GGDBF
{
	/// <summary>
	/// An HTTP <see cref="IGGDBFDataSource"/> implementation that uses the Refit <see cref="RefitHttpGGDBFDataSource"/> interface
	/// to source the data.
	/// </summary>
	public sealed class RefitHttpGGDBFDataSource : IGGDBFDataSource
	{
		/// <summary>
		/// The base URL for the requests.
		/// </summary>
		private string BaseUrl { get; }

		public RefitHttpGGDBFDataSource(string baseUrl)
		{
			if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));
			BaseUrl = baseUrl;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<TModelType>> RetrieveAllAsync<TModelType>(CancellationToken token = default) 
			where TModelType : class
		{
			throw new NotSupportedException($"TODO: Support retrieve all model type over HTTP");
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class
		{
			return await RestService
				.For<IGGDBFHttpNetworkClient>(BaseUrl)
				.RetrieveTableAsync<TPrimaryKeyType, TModelType>(typeof(TPrimaryKeyType).AssemblyQualifiedName, typeof(TModelType).AssemblyQualifiedName, token);
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			return await RestService
				.For<IGGDBFHttpNetworkClient>(BaseUrl)
				.RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(typeof(TPrimaryKeyType).AssemblyQualifiedName, typeof(TModelType).AssemblyQualifiedName, typeof(TSerializableModelType).AssemblyQualifiedName, token);
		}
	}
}
