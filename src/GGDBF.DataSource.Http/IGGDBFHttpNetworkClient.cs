using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace GGDBF
{
	//TODO: Doc
	public interface IGGDBFHttpNetworkClient
	{
		[Get("/api/GGDBF/{key}_{type}")]
		public Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType>([AliasAs("key")] string keyType, [AliasAs("type")] string modelType, CancellationToken token = default)
			where TModelType : class;

		[Get("/api/GGDBF/{key}_{type}/{derivedType}")]
		public Task<GGDBFTable<TPrimaryKeyType, TSerializableModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>([AliasAs("key")] string keyType, [AliasAs("type")] string modelType, [AliasAs("derivedType")] string derivedModelType, CancellationToken token = default)
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
			where TModelType : class;

		/// <summary>
		/// Reloads the remote data source forces the data to be up-to-date.
		/// </summary>
		/// <typeparam name="TGGDBFContextType">The context to refresh.</typeparam>
		/// <param name="contextType">The context type name to reload.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns></returns>
		[Post("/api/GGDBF/{context}/reload")]
		public Task ReloadContextAsync<TGGDBFContextType>([AliasAs("context")] string contextType, CancellationToken token = default)
			where TGGDBFContextType : class, IGGDBFContext;
	}
}
