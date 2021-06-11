using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class TableNameParser
	{
		public string Parse(ITypeSymbol type)
		{
			return (string)type
				.GetAttributeExact<TableAttribute>(true)
				.ConstructorArguments
				.First()
				.Value;
		}
	}
}
