using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GGDBF
{
	public sealed class UsingsEmitter : ISourceEmitter
	{
		private HashSet<string> RequestedNamespaces { get; } = new();

		private const string USING_DIRECTIVE_FORMAT_TEXT = "using {0};";

		public void Emit(StringBuilder builder, CancellationToken cancellationToken)
		{
			if (builder == null) throw new ArgumentNullException(nameof(builder));

			foreach (var entry in RequestedNamespaces)
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				builder.AppendFormat(USING_DIRECTIVE_FORMAT_TEXT, entry);
				builder.Append(Environment.NewLine);
			}

			if (cancellationToken.IsCancellationRequested)
				return;

			builder.Append(Environment.NewLine);
		}

		/// <summary>
		/// Adds the requested namespace to the using emitter.
		/// </summary>
		/// <param name="namespace">The namespace to add.</param>
		public void AddNamespace(string @namespace)
		{
			if (string.IsNullOrWhiteSpace(@namespace)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(@namespace));
			RequestedNamespaces.Add(@namespace);
		}

		public void AddNamespaces(IEnumerable<string> namespaces)
		{
			if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));

			foreach(var n in GGDBFConstants.DEFAULT_NAMESPACES)
				AddNamespace(n);
		}
	}
}
