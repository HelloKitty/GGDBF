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
			string tableName = (string)type
				.GetAttributeExact<TableAttribute>(true)
				.ConstructorArguments
				.First()
				.Value;

			if (String.IsNullOrWhiteSpace(tableName))
				return tableName;

			//Remove underscores from table names since they look terrible
			//in actual code.
			tableName = tableName.Replace("_", "");

			//There are cases where it might be a reserved word
			if (tableName == "Class" || tableName == "class")
				tableName = $"@{tableName}";

			return tableName;
		}

		public string ParseNameToStringLiteral(string tableName)
		{
			//Some strings may have @ due to reserved words
			tableName = tableName.Replace("@", "");

			return $"\"{tableName}\"";
		}
	}
}
