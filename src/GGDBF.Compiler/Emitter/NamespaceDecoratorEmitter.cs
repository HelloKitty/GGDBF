using System;
using System.Collections.Generic;
using System.Text;

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

		public void Emit(StringBuilder builder)
		{
			builder.Append($"namespace {NamespaceName}{Environment.NewLine}{{");
			Emitter.Emit(builder);
			builder.Append($"{Environment.NewLine}}}");
		}
	}
}
