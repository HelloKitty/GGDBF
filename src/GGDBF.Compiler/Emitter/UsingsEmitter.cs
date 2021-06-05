﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF.Compiler
{
	public sealed class UsingsEmitter : ISourceEmitter
	{
		private HashSet<string> RequestedNamespaces { get; } = new();

		private const string USING_DIRECTIVE_FORMAT_TEXT = "using {0};";

		public void Emit(StringBuilder builder)
		{
			if (builder == null) throw new ArgumentNullException(nameof(builder));

			foreach (var entry in RequestedNamespaces)
			{
				builder.AppendFormat(USING_DIRECTIVE_FORMAT_TEXT, entry);
				builder.Append("\n");
			}
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
	}
}