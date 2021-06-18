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
		[Get("/api/GGDBF/{key}_{type}")]
		public Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType>([AliasAs("key")] string keyType, [AliasAs("type")] string modelType, CancellationToken token = default)
			where TModelType : class;

		[Get("/api/GGDBF/{key}_{type}/{derivedType}")]
		public Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>([AliasAs("key")] string keyType, [AliasAs("type")] string modelType, [AliasAs("derivedType")] string derivedModelType, CancellationToken token = default)
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
			where TModelType : class;
	}
}
