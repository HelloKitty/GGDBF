using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GGDBF
{
	public static class ISourceEmitterExtensions
	{
		public static string Emit(this ISourceEmitter emitter, CancellationToken token)
		{
			if (emitter == null) throw new ArgumentNullException(nameof(emitter));

			var builder = new StringBuilder();
			emitter.Emit(builder, token);
			return builder.ToString();
		}
	}
}
