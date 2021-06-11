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

			builder.Append($"public static {ClassName} Instance {{ get; private set; }}");

			foreach (var entry in Properties)
				builder.Append($"public {CreatePropertyType(entry)} {entry.Name} {{ get; init; }}");

			AddInitializeMethod(builder);

			builder.Append($"\n}}");
		}

		private void AddInitializeMethod(StringBuilder builder)
		{
			builder.Append($"public static async Task Initialize({nameof(IGGDBFDataSource)} source){{");

			//Instance init
			builder.Append($"Instance = new()\n{{");

			foreach (var prop in Properties)
			{
				builder.Append($"{prop.Name} = (await source.{nameof(IGGDBFDataSource.RetrieveTableAsync)}<{new TablePrimaryKeyParser().Parse(prop.PropertyType)}, {prop.PropertyType}>()).TableData,\n");
			}

			builder.Append($"\n}};");

			//End method
			builder.Append($"\n}}");
		}

		private static string CreatePropertyType(PropertyDefinition entry)
		{
			return $"IReadOnlyDictionary<{new TablePrimaryKeyParser().Parse(entry.PropertyType).GetFriendlyName()}, {entry.PropertyType.GetFriendlyName()}>";
		}

		public void AddProperty(string name, INamedTypeSymbol type)
		{
			Properties.Add(new PropertyDefinition(name, type));
		}
	}
}
