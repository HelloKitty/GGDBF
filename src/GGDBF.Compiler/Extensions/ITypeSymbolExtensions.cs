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
			return type.GetMembers().Any(m => m.HasAttributeLike<ForeignKeyPropertyMissingHintAttribute>())
			       || type.GetMembers().Any(m => m.HasAttributeLike<ForeignKeyAttribute>())
			       || type.GetMembers().Any(m => m.HasAttributeLike<ForeignKeyHintAttribute>());
		}
	}
}
