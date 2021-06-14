using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public static class ITypeSymbolExtensions
	{
		public static bool HasForeignKeyDefined(this ITypeSymbol type)
		{
			return HasForeignKeyAttributes(type) || HasForeignCollection(type);
		}

		private static bool HasForeignCollection(ITypeSymbol type)
		{
			return type
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property && m.IsVirtual)
				.Cast<IPropertySymbol>()
				.Any(IsICollectionType);
		}

		public static bool IsICollectionType(this IPropertySymbol p)
		{
			return p.Type is INamedTypeSymbol symbol && symbol.IsGenericType && symbol.ConstructUnboundGenericType().Name == nameof(ICollection<object>);
		}

		private static bool HasForeignKeyAttributes(ITypeSymbol type)
		{
			return type
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property && m.IsVirtual)
				.Any(m => m.HasAttributeLike<ForeignKeyAttribute>() 
				          || m.HasAttributeLike<ForeignKeyPropertyMissingHintAttribute>() 
				          || m.HasAttributeLike<ForeignKeyHintAttribute>());
		}
	}
}
