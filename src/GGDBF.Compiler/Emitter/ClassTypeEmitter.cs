using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using GGDBF.Compiler;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public record PropertyDefinition(string Name, INamedTypeSymbol PropertyType);

	public sealed class ClassTypeEmitter : ISourceEmitter
	{
		private string ClassName { get; }

		private Accessibility ClassAccessibility { get; }

		private HashSet<PropertyDefinition> Properties { get; } = new();

		public ClassTypeEmitter(string className, Accessibility classAccessibility = Accessibility.NotApplicable)
		{
			if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(className));
			ClassName = className;
			ClassAccessibility = classAccessibility;
		}

		public void Emit(StringBuilder builder)
		{
			if (ClassAccessibility == Accessibility.NotApplicable)
				builder.Append($"partial class {ClassName}\n{{");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial class {ClassName}\n{{");

			foreach (var entry in Properties)
				builder.Append($"public {CreatePropertyType(entry)} {entry.Name} {{ get; }}");

			builder.Append($"\n}}");
		}

		private static string CreatePropertyType(PropertyDefinition entry)
		{
			return $"IReadOnlyDictionary<{DeterminePrimaryKeyType(entry).GetFriendlyName()}, {entry.PropertyType.GetFriendlyName()}>";
		}

		private static ITypeSymbol DeterminePrimaryKeyType(PropertyDefinition entry)
		{
			foreach (var prop in entry.PropertyType.GetMembers())
			{
				try
				{
					if(prop.Kind != SymbolKind.Property)
						continue;
					else if(prop.HasAttributeExact<KeyAttribute>() || prop.HasAttributeExact<DatabaseKeyHintAttribute>())
						return ((IPropertySymbol)prop).Type;
				}
				catch (Exception e)
				{
					throw new InvalidOperationException($"Failed to parse PK from Type: {entry.PropertyType.Name} on Property: {prop.Name} Reason: {e.Message} Stack: {e.StackTrace.Split('\n').LastOrDefault()}", e);
				}
			}

			throw new InvalidOperationException($"Failed to deduce PK from ModelType: {entry.PropertyType.Name}");
		}

		public void AddProperty(string name, INamedTypeSymbol type)
		{
			Properties.Add(new PropertyDefinition(name, type));
		}
	}
}
