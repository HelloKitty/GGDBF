using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Glader.Essentials;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
	}
}