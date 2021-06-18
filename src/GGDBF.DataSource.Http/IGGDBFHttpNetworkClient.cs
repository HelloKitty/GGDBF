using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace GGDBF
{
	public interface IGGDBFHttpNetworkClient<TPrimaryKeyType, TModelType>
	{
		[Get("/api/GGDBF/{name}")]
		public Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync([AliasAs("name")] string tableName, CancellationToken token = default);

		[Get("/api/GGDBF/{name}/{derivedType}")]
		public Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync([AliasAs("name")] string tableName, [AliasAs("derivedType")] string modelType, CancellationToken token = default);
	}
}
