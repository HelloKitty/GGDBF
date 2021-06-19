using System;
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

		public ContextReflectionGGDBFDataSource(IGGDBFDataSource source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
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
			TGGDBFContextType context = await GetContext(Source);

			//Total hack but the best way to get the data I guess.
			var modelDictionary = (IReadOnlyDictionary<TPrimaryKeyType, TModelType>)typeof(TGGDBFContextType)
				.GetProperties(BindingFlags.Public)
				.First(p => p.PropertyType == typeof(IReadOnlyDictionary<TPrimaryKeyType, TModelType>))
				.GetValue(context);

			return new GGDBFTable<TPrimaryKeyType, TModelType>()
			{
				Version = GGDBFHelpers.GetContextVersion<TGGDBFContextType>(),
				TableName = GGDBFHelpers.GetTableName<TModelType>(),
				TableData = modelDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
			};
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) where TModelType : class where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			//Assume the datasource has handling the serializable types and such. Not much more we can do here.
			return await RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(config, token);
		}

		private async Task<TGGDBFContextType> GetContext(IGGDBFDataSource source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//TODO: This could load the context multiple times due to race conditions (but should be ok)
			if(!GGDBFHelpers.IsContextInitialized<TGGDBFContextType>())
				await GGDBFHelpers.CallInitialize<TGGDBFContextType>(source);

			return GGDBFHelpers.GetInstance<TGGDBFContextType>();
		}
	}
}
