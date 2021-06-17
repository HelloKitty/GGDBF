using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class ForeignKeyContainingPropertyNameParser
	{
		public string Parse(INamedTypeSymbol contextType, ITypeSymbol propertyType)
		{
			if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));
			return $"{contextType.Name}_{ComputeSerializableTypeName(contextType, propertyType)}";
		}

		public string ParseNonGeneric(INamedTypeSymbol contextType, ITypeSymbol propertyType)
		{
			return $"{contextType.Name}_{propertyType.Name}";
		}

		private static string ComputeSerializableTypeName(INamedTypeSymbol contextType, ITypeSymbol prop)
		{
			if (contextType == null) throw new ArgumentNullException(nameof(contextType));
			if (prop == null) throw new ArgumentNullException(nameof(prop));

			if (!contextType.IsGenericType)
				return $"{prop.Name}";

			//If generic context then serializable type
			//must carry its parameters
			return new GenericTypeBuilder(contextType.TypeParameters.ToArray()).Build(prop.Name);
		}
	}
}
