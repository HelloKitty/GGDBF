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

	public sealed class ContextClassTypeEmitter : BaseClassTypeEmitter, ISourceEmitter
	{
		private HashSet<PropertyDefinition> Properties { get; } = new();

		public ContextClassTypeEmitter(string className, Accessibility classAccessibility = Accessibility.NotApplicable)
			: base(className, classAccessibility)
		{

		}

		public void AddProperty(string name, INamedTypeSymbol type)
		{
			Properties.Add(new PropertyDefinition(name, type));
		}

		public override void Emit(StringBuilder builder)
		{
			//First we build the interface for the context type
			builder.Append($"[{nameof(GeneratedCodeAttribute)}(\"GGDBF\", \"{typeof(GGDBFTable<object, object>).Assembly.GetName().Version}\")]{Environment.NewLine}");
			builder.Append($"{ClassAccessibility.ToString().ToLower()} interface I{ClassName}{Environment.NewLine}{{");

			foreach(var entry in Properties)
				builder.Append($"public {CreatePropertyType(entry)} {new TableNameParser().Parse(entry.PropertyType)} {{ get; init; }}{Environment.NewLine}{Environment.NewLine}");

			builder.Append($"}}{Environment.NewLine}{Environment.NewLine}");

			//Class time!
			builder.Append($"[{nameof(GeneratedCodeAttribute)}(\"GGDBF\", \"{typeof(GGDBFTable<object, object>).Assembly.GetName().Version}\")]{Environment.NewLine}");
			if (ClassAccessibility == Accessibility.NotApplicable)
				builder.Append($"partial class {ClassName} : I{ClassName}{Environment.NewLine}{{");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial class {ClassName} : I{ClassName}{Environment.NewLine}{{");

			builder.Append($"public static {ClassName} Instance {{ get; private set; }}{Environment.NewLine}{Environment.NewLine}");

			foreach (var entry in Properties)
				builder.Append($"public {CreatePropertyType(entry)} {new TableNameParser().Parse(entry.PropertyType)} {{ get; init; }}{Environment.NewLine}{Environment.NewLine}");

			AddInitializeMethod(builder);

			builder.Append($"{Environment.NewLine}}}");
		}

		private void AddInitializeMethod(StringBuilder builder)
		{
			builder.Append($"public static async Task Initialize({nameof(IGGDBFDataSource)} source){{");

			//Instance init
			builder.Append($"Instance = new(){Environment.NewLine}{{");

			foreach (var prop in Properties)
			{
				//We must know the name of the table at compile time to emit the
				//proper config so we don't need to use reflection at runtime
				var tableName = new TableNameParser().Parse(prop.PropertyType);
				builder.Append(Environment.NewLine);
				builder.Append($"{prop.Name} = await source.{nameof(IGGDBFDataSourceExtensions.RetrieveTableAsync)}<{CreateRetrieveGenericParameters(prop)}>({CreateTableConfig(tableName, new TablePrimaryKeyParser().Parse(prop.PropertyType).ToString(), ComputeTypeName(prop))}),");
			}

			builder.Append($"{Environment.NewLine}}};");

			//End method
			builder.Append($"{Environment.NewLine}}}");
		}

		private string CreateRetrieveGenericParameters(PropertyDefinition prop)
		{
			if (prop.PropertyType.HasForeignKeyDefined())
				return $"{new TablePrimaryKeyParser().Parse(prop.PropertyType)}, {ComputeTypeName(prop)}, {new ForeignKeyContainingPropertyNameParser().Parse(ClassName, prop.PropertyType)}";

			return $"{new TablePrimaryKeyParser().Parse(prop.PropertyType)}, {ComputeTypeName(prop)}";
		}

		private static string ComputeTypeName(PropertyDefinition prop)
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
	}
}
