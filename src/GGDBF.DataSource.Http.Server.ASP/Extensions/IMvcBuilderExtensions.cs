using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Glader.Essentials;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace GGDBF
{
	public static class IMvcBuilderExtensions
	{
		/// <summary>
		/// Registers the general <see cref="GGDBFContentController"/> with the MVC
		/// controllers. See controller documentation for what it does and how it works.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IMvcBuilder RegisterGGDBFController(this IMvcBuilder builder)
		{
			if(builder == null) throw new ArgumentNullException(nameof(builder));

			return builder
				.RegisterController<GGDBFContentController>();
		}

#if NETCOREAPP3_1_OR_GREATER
		/// <summary>
		/// Registers the GGDBF JSON serializers.
		/// </summary>
		/// <param name="options">JSON serializer options.</param>
		/// <returns>The options.</returns>
		public static MvcNewtonsoftJsonOptions RegisterGGDBFSerializers(this MvcNewtonsoftJsonOptions options)
		{
			if (options.SerializerSettings.Converters != null && options.SerializerSettings.Converters.Any())
			{
				options.SerializerSettings.Converters.Add(new GGDBFComplexDictionaryJsonConverter());
				options.SerializerSettings.Converters.Add(new SerializableGGDBFCollectionJsonConverter());
			}
			else
				options.SerializerSettings.Converters = new List<JsonConverter>() { new GGDBFComplexDictionaryJsonConverter(), new SerializableGGDBFCollectionJsonConverter() };

			return options;
		}
#endif
	}
}