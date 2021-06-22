using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Refit;

namespace GGDBF
{
	public record RefitHttpGGDBFDataSourceOptions(bool RefreshOnFirstQuery = false, RefitSettings Settings = null);

	/// <summary>
	/// An HTTP <see cref="IGGDBFDataSource"/> implementation that uses the Refit <see cref="IGGDBFHttpNetworkClient"/> interface
	/// to source the data.
	/// </summary>
	public sealed class RefitHttpGGDBFDataSource<TGGDBFContextType> : IGGDBFDataSource
		where TGGDBFContextType : class, IGGDBFContext
	{
		/// <summary>
		/// The base URL for the requests.
		/// </summary>
		private string BaseUrl { get; }

		public RefitHttpGGDBFDataSourceOptions Options { get; }

		private bool FirstQuery { get; set; } = true;

		public RefitHttpGGDBFDataSource(string baseUrl, RefitHttpGGDBFDataSourceOptions options = null)
		{
			if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(baseUrl));
			BaseUrl = baseUrl;
			Options = options ?? new RefitHttpGGDBFDataSourceOptions();
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
			await ReloadIfRequiredAsync(token);

			return await CreateServiceClient()
				.RetrieveTableAsync<TPrimaryKeyType, TModelType>(typeof(TPrimaryKeyType).AssemblyQualifiedName, typeof(TModelType).AssemblyQualifiedName, token);
		}

		private IGGDBFHttpNetworkClient CreateServiceClient() 
		{
			//Creates a Refit client that can understand complex dictionary key type serialization.
			return RestService
				.For<IGGDBFHttpNetworkClient>(BaseUrl, CreateRefitSettings());
		}

		private RefitSettings CreateRefitSettings()
		{
			//Important that we combine the Refit settings if they already exist.
			var settings = Options.Settings ?? new RefitSettings();

			settings.ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings()
			{
				Converters = new List<JsonConverter>()
				{
					new GGDBFComplexDictionaryJsonConverter()
				}
			});

			return settings;
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			await ReloadIfRequiredAsync(token);

			var table = await CreateServiceClient()
				.RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(typeof(TPrimaryKeyType).AssemblyQualifiedName, typeof(TModelType).AssemblyQualifiedName, typeof(TSerializableModelType).AssemblyQualifiedName, token);
			return table.ConvertFrom<TPrimaryKeyType, TModelType, TSerializableModelType>();
		}

		/// <inheritdoc />
		public async Task ReloadAsync(CancellationToken token = default)
		{
			await RestService
				.For<IGGDBFHttpNetworkClient>(BaseUrl)
				.ReloadContextAsync<TGGDBFContextType>(typeof(TGGDBFContextType).AssemblyQualifiedName, token);
		}

		private async Task ReloadIfRequiredAsync(CancellationToken token = default) 
		{
			if (Options.RefreshOnFirstQuery && FirstQuery)
			{
				await ReloadAsync(token);
				FirstQuery = true;
			}
		}
	}
}
