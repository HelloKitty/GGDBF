using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace GGDBF
{
	public interface IGGDBFHttpNetworkClient
	{
		[Get("/api/GGDBF/{key}_{name}")]
		public Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType>([AliasAs("key")] string keyType, [AliasAs("name")] string tableName, CancellationToken token = default);

		[Get("/api/GGDBF/{key}_{name}/{derivedType}")]
		public Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>([AliasAs("key")] string keyType, [AliasAs("name")] string tableName, [AliasAs("derivedType")] string modelType, CancellationToken token = default)
			where TSerializableModelType : class, TModelType;
	}
}
