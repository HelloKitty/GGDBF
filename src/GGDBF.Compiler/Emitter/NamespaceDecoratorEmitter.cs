using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GGDBF
{
	public sealed class NamespaceDecoratorEmitter : ISourceEmitter
	{
		private ISourceEmitter Emitter { get; }

		private string NamespaceName { get; }

		public NamespaceDecoratorEmitter(ISourceEmitter emitter, string namespaceName)
		{
			Emitter = emitter ?? throw new ArgumentNullException(nameof(emitter));
			NamespaceName = namespaceName ?? throw new ArgumentNullException(nameof(namespaceName));
		}

		public void Emit(StringBuilder builder, CancellationToken token)
		{
			if (token.IsCancellationRequested)
				return;

			builder.Append($"namespace {NamespaceName}{Environment.NewLine}{{");
			Emitter.Emit(builder, token);
			builder.Append($"{Environment.NewLine}}}");
		}
	}
}
