﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class TablePrimaryKeyParser
	{
		public ITypeSymbol Parse(ITypeSymbol type)
		{
			foreach(var prop in type.GetMembers())
			{
				try
				{
					if(prop.Kind != SymbolKind.Property)
						continue;
					else if(prop.HasAttributeExact<KeyAttribute>() || prop.HasAttributeExact<DatabaseKeyHintAttribute>())
						return ((IPropertySymbol)prop).Type;
				}
				catch(Exception e)
				{
					throw new InvalidOperationException($"Failed to parse PK from Type: {type.Name} on Property: {prop.Name} Reason: {e.Message} Stack: {e.StackTrace.Split('\n').LastOrDefault()}", e);
				}
			}

			throw new InvalidOperationException($"Failed to deduce PK from ModelType: {type.Name}");
		}
	}
}