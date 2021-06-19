﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace GGDBF
{
	public record RefitHttpGGDBFDataSourceOptions(bool RefreshOnFirstQuery = false);

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
			await ReloadIfRequiredAsync<TPrimaryKeyType, TModelType>(token);

			return await RestService
				.For<IGGDBFHttpNetworkClient>(BaseUrl)
				.RetrieveTableAsync<TPrimaryKeyType, TModelType>(typeof(TPrimaryKeyType).AssemblyQualifiedName, typeof(TModelType).AssemblyQualifiedName, token);
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			await ReloadIfRequiredAsync<TPrimaryKeyType, TModelType>(token);

			return await RestService
				.For<IGGDBFHttpNetworkClient>(BaseUrl)
				.RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(typeof(TPrimaryKeyType).AssemblyQualifiedName, typeof(TModelType).AssemblyQualifiedName, typeof(TSerializableModelType).AssemblyQualifiedName, token);
		}

		/// <inheritdoc />
		public async Task ReloadAsync(CancellationToken token = default)
		{
			await RestService
				.For<IGGDBFHttpNetworkClient>(BaseUrl)
				.ReloadContextAsync<TGGDBFContextType>(typeof(TGGDBFContextType).AssemblyQualifiedName, token);
		}

		private async Task ReloadIfRequiredAsync<TPrimaryKeyType, TModelType>(CancellationToken token = default) 
			where TModelType : class
		{
			if (Options.RefreshOnFirstQuery && FirstQuery)
			{
				await ReloadAsync(token);
				FirstQuery = true;
			}
		}
	}
}
