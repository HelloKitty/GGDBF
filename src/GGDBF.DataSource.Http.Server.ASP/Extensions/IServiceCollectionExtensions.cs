using System;
using System.Collections.Generic;
using System.Text;
using GGDBF;
using GGDBF.Generator;
using Microsoft.Extensions.DependencyInjection;

namespace Glader.ASP.RPG
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Registers the specified source and converter combined as a <see cref="IGGDBFDataSource"/> registeration.
		/// </summary>
		/// <param name="services">Service container.</param>
		/// <returns></returns>
		public static IServiceCollection RegisterGGDBFContentServices<TGGDBFDataSourceType, TGGDBFContextType>(this IServiceCollection services)
			where TGGDBFDataSourceType : class, IGGDBFDataSource 
			where TGGDBFContextType : class, IGGDBFContext
		{
			if(services == null) throw new ArgumentNullException(nameof(services));

			services.AddTransient<TGGDBFDataSourceType>();

			//TODO: Maybe put this in the RegisterGGDBF
			//GGDBF requires a datasource registered
			services.AddTransient<IGGDBFDataSource>(provider =>
			{
				return new ContextReflectionGGDBFDataSource<TGGDBFContextType>(provider.GetService<TGGDBFDataSourceType>());
			});

			return services;
		}
	}
}
