using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public static class IGGDBFDataSourceExtensions
	{
		public static async Task<IReadOnlyDictionary<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType>(this IGGDBFDataSource source, TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class
		{
			return (await source.RetrieveFullTableAsync(config, token)).ToReadOnly();
		}

		public static async Task<IReadOnlyDictionary<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(this IGGDBFDataSource source, TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default)
			where TModelType : class 
			where TSerializableModelType : TModelType
		{
			return (await source.RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(config, token)).ToReadOnly();
		}
	}
}
