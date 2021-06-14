using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	public static class ISourceEmitterExtensions
	{
		public static string Emit(this ISourceEmitter emitter)
		{
			if (emitter == null) throw new ArgumentNullException(nameof(emitter));

			var builder = new StringBuilder();
			emitter.Emit(builder);
			return builder.ToString();
		}
	}
}
