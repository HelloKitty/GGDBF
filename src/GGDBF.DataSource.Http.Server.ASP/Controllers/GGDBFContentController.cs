using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GGDBF
{
	[Route("api/GGDBF")]
	public sealed class GGDBFContentController : BaseGladerController, IGGDBFHttpNetworkClient
	{
		private IGGDBFDataSource DataSource { get; }

		public GGDBFContentController(ILogger<BaseGladerController> logger, IGGDBFDataSource dataSource) 
			: base(logger)
		{
			DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
		}

		//[Get("/api/GGDBF/{key}_{name}")]
		[ProducesJson]
		[HttpGet("{key}_{type}")]
		public async Task<IActionResult> GetAsync([FromRoute(Name = "key")] string keyType, [FromRoute(Name = "type")] string modelType, CancellationToken token = default)
		{
			if (modelType == null) throw new ArgumentNullException(nameof(modelType));

			//WARNING: Not safe to expose in production https://stackoverflow.com/questions/23895563/is-it-safe-to-call-type-gettype-with-an-untrusted-type-name
			var resultAwaitable = (Task)GetType().GetMethod(nameof(RetrieveTableAsync), new Type[]{ typeof(string), typeof(string), typeof(CancellationToken)})
				.MakeGenericMethod(ResolveTypeForGGDBF(keyType), ResolveTypeForGGDBF(modelType))
				.Invoke(this, new object[] { keyType, modelType, token });

			await resultAwaitable;
			var result = resultAwaitable
				.GetType()
				.GetProperty(nameof(Task<object>.Result))
				.GetValue(resultAwaitable);

			return Json(result);
		}

		[ProducesJson]
		[HttpGet("{key}_{type}/{derivedType}")]
		public async Task<IActionResult> GetAsync([FromRoute(Name = "key")] string keyType, [FromRoute(Name = "type")] string modelType, [FromRoute(Name = "derivedType")] string derivedType, CancellationToken token = default)
		{
			if (keyType == null) throw new ArgumentNullException(nameof(keyType));
			if (modelType == null) throw new ArgumentNullException(nameof(modelType));

			//WARNING: Not safe to expose in production https://stackoverflow.com/questions/23895563/is-it-safe-to-call-type-gettype-with-an-untrusted-type-name
			var resultAwaitable = (Task)GetType().GetMethod(nameof(RetrieveTableAsync), new Type[] { typeof(string), typeof(string), typeof(string), typeof(CancellationToken) })
				.MakeGenericMethod(ResolveTypeForGGDBF(keyType), ResolveTypeForGGDBF(modelType), ResolveTypeForGGDBF(derivedType))
				.Invoke(this, new object[] { keyType, modelType, derivedType, token });

			await resultAwaitable;
			var result = resultAwaitable
				.GetType()
				.GetProperty(nameof(Task<object>.Result))
				.GetValue(resultAwaitable);

			return Json(result);
		}

		//[Post("/api/GGDBF/{context}/reload")]
		[HttpPost("{context}/reload")]
		public async Task<IActionResult> ReloadAsync([FromRoute(Name = "context")] string contextType, CancellationToken token = default)
		{
			//WARNING: Not safe to expose in production https://stackoverflow.com/questions/23895563/is-it-safe-to-call-type-gettype-with-an-untrusted-type-name
			var resultAwaitable = (Task)GetType().GetMethod(nameof(ReloadAsync), new Type[] { typeof(string), typeof(CancellationToken) })
				.MakeGenericMethod(ResolveTypeForGGDBF(contextType))
				.Invoke(this, new object[] { contextType, token });

			await resultAwaitable;

			return Ok();
		}

		private static Type ResolveTypeForGGDBF(string modelType)
		{
			return Type.GetType(modelType, GGDBFAssemblyResolver, null);
		}

		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType>(string keyType, string tableName, CancellationToken token = default) 
			where TModelType : class
		{
			//We can ignore parameters since this is generic and we're implementing using the generics and not the text types
			//We can ignore parameters since this is generic and we're implementing using the generics and not the text types
			return await DataSource
				.RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(new TableRetrievalConfig<TPrimaryKeyType, TModelType>(), token);
		}

		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(string keyType, string tableName, string modelType, CancellationToken token = default) 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
			where TModelType : class
		{
			//We can ignore parameters since this is generic and we're implementing using the generics and not the text types
			return await DataSource
				.RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(new TableRetrievalConfig<TPrimaryKeyType, TModelType>(), token);
		}

		public async Task ReloadContextAsync<TGGDBFContextType>(string contextType, CancellationToken token = default) 
			where TGGDBFContextType : class, IGGDBFContext
		{
			await DataSource.ReloadAsync(token);
		}

		//See: https://stackoverflow.com/a/33277079
		//This resolver method will ignore version, cultures and keys to make loading more reasonable.
		private static System.Reflection.Assembly GGDBFAssemblyResolver(System.Reflection.AssemblyName assemblyName)
		{
			assemblyName.Version = null;
			assemblyName.SetPublicKey(null);
			assemblyName.SetPublicKeyToken(null);
			assemblyName.CultureInfo = null;
			assemblyName.CultureName = null;

			return System.Reflection.Assembly.Load(assemblyName);
		}
	}
}
