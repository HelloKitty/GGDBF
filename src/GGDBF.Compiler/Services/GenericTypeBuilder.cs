using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class GenericTypeBuilder
	{
		private ITypeParameterSymbol[] TypeArgs { get; }

		public GenericTypeBuilder(ITypeParameterSymbol[] typeArgs)
		{
			TypeArgs = typeArgs ?? throw new ArgumentNullException(nameof(typeArgs));
		}

		public string Build(string className)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append($"{className}<");

			for(int i = 0; i < TypeArgs.Length; i++)
			{
				builder.Append(TypeArgs[i].Name);
				if(i < TypeArgs.Length - 1)
					builder.Append(", ");
			}

			builder.Append($">");
			return builder.ToString();
		}
	}
}
