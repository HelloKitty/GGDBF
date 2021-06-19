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
		public static IServiceCollection RegisterGGDBFContentServices<TGGDBFDataSourceType, TGGDBFConverterType>(this IServiceCollection services)
			where TGGDBFDataSourceType : class, IGGDBFDataSource 
			where TGGDBFConverterType : class, IGGDBFDataConverter
		{
			if(services == null) throw new ArgumentNullException(nameof(services));

			services.AddTransient<TGGDBFDataSourceType>();
			services.AddTransient<TGGDBFConverterType>();

			//TODO: Maybe put this in the RegisterGGDBF
			//GGDBF requires a datasource registered
			services.AddTransient<IGGDBFDataSource>(provider =>
			{
				return new ConverterDataSourceGGDBFAdapter(provider.GetService<TGGDBFDataSourceType>(), provider.GetService<TGGDBFConverterType>());
			});

			return services;
		}
	}
}
