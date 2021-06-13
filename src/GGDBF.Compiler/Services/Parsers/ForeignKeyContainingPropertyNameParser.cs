using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class ForeignKeyContainingPropertyNameParser
	{
		public string Parse(string contextTypeName, ITypeSymbol propertyType)
		{
			if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));
			return $"{contextTypeName}_{ComputeSerializableTypeName(propertyType)}";
		}

		private static string ComputeSerializableTypeName(ITypeSymbol prop)
		{
			if (prop == null) throw new ArgumentNullException(nameof(prop));
			return $"{prop.Name}";
		}
	}
}
