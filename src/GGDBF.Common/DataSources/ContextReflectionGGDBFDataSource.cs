﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public sealed class ContextReflectionGGDBFDataSource<TGGDBFContextType> : IGGDBFDataSource
		where TGGDBFContextType : IGGDBFContext
	{
		private IGGDBFDataSource Source { get; }

		private IGGDBFDataConverter Converter { get; }

		public ContextReflectionGGDBFDataSource(IGGDBFDataSource source, IGGDBFDataConverter converter)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			Converter = converter ?? throw new ArgumentNullException(nameof(converter));
		}

		/// <inheritdoc />
		public async Task<IEnumerable<TModelType>> RetrieveAllAsync<TModelType>(CancellationToken token = default) 
			where TModelType : class
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) where TModelType : class
		{
			TGGDBFContextType context = await GetContext(Source, token);

			try
			{
				//Total hack but the best way to get the data I guess.
				var modelDictionary = (IReadOnlyDictionary<TPrimaryKeyType, TModelType>)typeof(TGGDBFContextType)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
					.First(p => p.PropertyType == typeof(IReadOnlyDictionary<TPrimaryKeyType, TModelType>))
					.GetValue(context);

				return new GGDBFTable<TPrimaryKeyType, TModelType>()
				{
					Version = GGDBFHelpers.GetContextVersion<TGGDBFContextType>(),
					TableName = GGDBFHelpers.GetTableName<TModelType>(),
					TableData = modelDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
				};
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to {nameof(RetrieveFullTableAsync)} for Key: {typeof(TPrimaryKeyType)} Model: {typeof(TModelType)}", e);
			}
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) where TModelType : class where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			var table = await RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(config, token);

			return new GGDBFTable<TPrimaryKeyType, TModelType>()
			{
				Version = table.Version,
				TableName = table.TableName,
				TableData = table.TableData
					.ToDictionary(t => t.Key, t => (TModelType)Converter.Convert<TModelType, TSerializableModelType>(t.Value))
			};
		}

		public async Task ReloadAsync(CancellationToken token = default)
		{
			await Source.ReloadAsync(token);
			await GGDBFHelpers.CallInitialize<TGGDBFContextType>(Source, token, true);
		}

		private async Task<TGGDBFContextType> GetContext(IGGDBFDataSource source, CancellationToken token = default)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			if(!GGDBFHelpers.IsContextInitialized<TGGDBFContextType>())
				await GGDBFHelpers.CallInitialize<TGGDBFContextType>(source, token);

			return GGDBFHelpers.GetInstance<TGGDBFContextType>();
		}
	}
}
