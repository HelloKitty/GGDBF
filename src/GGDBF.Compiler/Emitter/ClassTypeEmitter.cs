using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
			//First we build the interface for the context type
			builder.Append($"[{nameof(GeneratedCodeAttribute)}(\"GGDBF\", \"{typeof(GGDBFTable<object, object>).Assembly.GetName().Version}\")]\n");
			builder.Append($"{ClassAccessibility.ToString().ToLower()} interface I{ClassName}\n{{");

			foreach(var entry in Properties)
				builder.Append($"public {CreatePropertyType(entry)} {entry.Name} {{ get; init; }}\n\n");

			builder.Append($"\n}}\n\n");

			//Class time!
			builder.Append($"[{nameof(GeneratedCodeAttribute)}(\"GGDBF\", \"{typeof(GGDBFTable<object, object>).Assembly.GetName().Version}\")]\n");
			if (ClassAccessibility == Accessibility.NotApplicable)
				builder.Append($"partial class {ClassName} : I{ClassName}\n{{");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial class {ClassName} : I{ClassName}\n{{");

			builder.Append($"public static {ClassName} Instance {{ get; private set; }}\n\n");

			foreach (var entry in Properties)
				builder.Append($"public {CreatePropertyType(entry)} {entry.Name} {{ get; init; }}\n\n");

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
				//We must know the name of the table at compile time to emit the
				//proper config so we don't need to use reflection at runtime
				var tableName = new TableNameParser().Parse(prop.PropertyType);
				builder.Append("\n");
				builder.Append($"{prop.Name} = await source.{nameof(IGGDBFDataSourceExtensions.RetrieveTableAsync)}<{CreateRetrieveGenericParameters(prop)}>({CreateTableConfig(tableName, new TablePrimaryKeyParser().Parse(prop.PropertyType).ToString(), ComputeSerializableTypeName(prop))}),");
			}

			builder.Append($"\n}};");

			//End method
			builder.Append($"\n}}");
		}

		private string CreateRetrieveGenericParameters(PropertyDefinition prop)
		{
			if (prop.PropertyType.HasForeignKeyDefined())
				return $"{new TablePrimaryKeyParser().Parse(prop.PropertyType)}, {ComputeSerializableTypeName(prop)}, {new ForeignKeyContainingPropertyNameParser().Parse(ClassName, prop.PropertyType)}";

			return $"{new TablePrimaryKeyParser().Parse(prop.PropertyType)}, {ComputeSerializableTypeName(prop)}";
		}

		private static string ComputeSerializableTypeName(PropertyDefinition prop)
		{
			return $"{prop.PropertyType.Name}";
		}

		private string CreateTableConfig(string tableName, string primaryKeyTypeString, string modelTypeString)
		{
			if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tableName));
			return $"new {nameof(NameOverrideTableRetrievalConfig<object, object>)}<{primaryKeyTypeString}, {modelTypeString}>(\"{tableName}\")";
		}

		private static string CreatePropertyType(PropertyDefinition entry)
		{
			return $"IReadOnlyDictionary<{new TablePrimaryKeyParser().Parse(entry.PropertyType)}, {entry.PropertyType.GetFriendlyName()}>";
		}

		public void AddProperty(string name, INamedTypeSymbol type)
		{
			Properties.Add(new PropertyDefinition(name, type));
		}
	}
}
