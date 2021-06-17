using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public record PropertyDefinition(string Name, INamedTypeSymbol PropertyType, bool IsRuntimeUnbounded);

	public sealed class ContextClassTypeEmitter : BaseClassTypeEmitter, ISourceEmitter
	{
		private HashSet<PropertyDefinition> Properties { get; } = new();

		private INamedTypeSymbol OriginalContextSymbol { get; }

		public ContextClassTypeEmitter(string className, INamedTypeSymbol originalContextSymbol, Accessibility classAccessibility = Accessibility.NotApplicable)
			: base(className, classAccessibility)
		{
			OriginalContextSymbol = originalContextSymbol ?? throw new ArgumentNullException(nameof(originalContextSymbol));
		}

		public void AddProperty(string name, INamedTypeSymbol type, bool isRuntimeUnbounded = false)
		{
			Properties.Add(new PropertyDefinition(name, type, isRuntimeUnbounded));
		}

		public override void Emit(StringBuilder builder)
		{
			//First we build the interface for the context type
			AppendGeneratedCodeAttribute(builder);
			EmitContextInterface(builder);
			
			//We emit even unbounded generic models on to the interface
			foreach(var entry in Properties)
				EmitTableProperty(builder, entry);

			builder.Append($"}}{Environment.NewLine}{Environment.NewLine}");

			//Class time!
			AppendGeneratedCodeAttribute(builder);
			if (ClassAccessibility == Accessibility.NotApplicable)
				builder.Append($"partial class {ComputeContextTypeName()} : {EmitInterfaceTypeName()}{Environment.NewLine}");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial class {ComputeContextTypeName()} : {EmitInterfaceTypeName()}");

			//Handle possible generic type parameters
			CalculatePossibleTypeConstraints(OriginalContextSymbol, builder);

			builder.Append($"{Environment.NewLine}{{");

			builder.Append($"public static {ComputeContextTypeName()} Instance {{ get; private set; }}{Environment.NewLine}{Environment.NewLine}");

			//Don't include any open generic property types. Any generic model fields must be included manually.
			foreach(var entry in Properties.Where(p => !p.IsRuntimeUnbounded && !p.PropertyType.IsUnboundGenericType))
				EmitTableProperty(builder, entry);

			AddInitializeMethod(builder);

			builder.Append($"{Environment.NewLine}}}");
		}

		private string ComputeContextTypeName()
		{
			if (!OriginalContextSymbol.IsGenericType)
				return ClassName;
			else
			{
				return new GenericTypeBuilder(OriginalContextSymbol.TypeParameters.ToArray())
					.Build(ClassName);
			}
		}

		private StringBuilder EmitTableProperty(StringBuilder builder, PropertyDefinition entry)
		{
			return builder.Append($"public {CreatePropertyType(entry)} {new TableNameParser().Parse(entry.PropertyType)} {{ get; init; }}{Environment.NewLine}{Environment.NewLine}");
		}

		private StringBuilder EmitContextInterface(StringBuilder builder)
		{
			//If the context is generic then the interface should be generic too.
			if (!OriginalContextSymbol.IsGenericType)
				return builder.Append($"{ClassAccessibility.ToString().ToLower()} interface I{ClassName} : {nameof(IGGDBFContext)}{Environment.NewLine}{{");
				
			builder.Append($"{ClassAccessibility.ToString().ToLower()} interface ");

			EmitInterfaceTypeName(builder);

			builder.Append($" : {nameof(IGGDBFContext)}");

			//Handle possible generic type parameters
			CalculatePossibleTypeConstraints(OriginalContextSymbol, builder);

			return builder.Append($"{Environment.NewLine}{{");
		}

		private string EmitInterfaceTypeName()
		{
			StringBuilder builder = new StringBuilder();
			EmitInterfaceTypeName(builder);
			return builder.ToString();
		}

		private void EmitInterfaceTypeName(StringBuilder builder)
		{
			if (!OriginalContextSymbol.IsGenericType)
			{
				builder.Append($"I{ClassName}");
				return;
			}
			
			builder.Append(new GenericTypeBuilder(OriginalContextSymbol.TypeParameters.ToArray())
				.Build($"I{ClassName}"));
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

				string typeName = GetModelTypeName(prop);

				builder.Append($"{prop.Name} = await source.{nameof(IGGDBFDataSourceExtensions.RetrieveTableAsync)}<{CreateRetrieveGenericParameters(prop)}>({CreateTableConfig(tableName, RetrievePropertyPrimaryKey(prop), typeName)}),");
			}

			builder.Append($"{Environment.NewLine}}};");

			//End method
			builder.Append($"{Environment.NewLine}}}");
		}

		private string GetModelTypeName(PropertyDefinition prop)
		{
			return !prop.PropertyType.IsUnboundGenericType ? ComputeTypeName(prop) : ((INamedTypeSymbol) GetMatchingContextProperty(prop).Type).TypeArguments.Last().GetFriendlyName();
		}

		private string RetrievePropertyPrimaryKey(PropertyDefinition prop)
		{
			//Idea here is to look at the original Context and see if the property is defined. If it is we should
			//take the key text the user manually put and use it as the primary key.
			if (prop.PropertyType.IsUnboundGenericType)
				return ((INamedTypeSymbol) GetMatchingContextProperty(prop).Type)
					.TypeArguments
					.First()
					.Name;

			return new TablePrimaryKeyParser().Parse(prop.PropertyType).ToString();
		}

		private string CreateRetrieveGenericParameters(PropertyDefinition prop)
		{
			string typeName = GetModelTypeName(prop);

			//TODO: This doesn't work for unbound generic types. The rest of the class does.
			if (prop.PropertyType.HasForeignKeyDefined())
				return $"{RetrievePropertyPrimaryKey(prop)}, {typeName}, {new ForeignKeyContainingPropertyNameParser().Parse(OriginalContextSymbol, prop.PropertyType)}";

			return $"{RetrievePropertyPrimaryKey(prop)}, {typeName}";
		}

		private string CreateTableConfig(string tableName, string primaryKeyTypeString, string modelTypeString)
		{
			if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tableName));
			return $"new {nameof(NameOverrideTableRetrievalConfig<object, object>)}<{primaryKeyTypeString}, {modelTypeString}>({new TableNameParser().ParseNameToStringLiteral(tableName)})";
		}

		private string CreatePropertyType(PropertyDefinition entry)
		{
			//Kinda hacky, but quick way to add support for generic types.
			if (entry.PropertyType.IsUnboundGenericType)
				return GetMatchingContextProperty(entry).Type.ToDisplayString();

			return $"IReadOnlyDictionary<{RetrievePropertyPrimaryKey(entry)}, {ComputeTypeName(entry)}>";
		}

		private IPropertySymbol GetMatchingContextProperty(PropertyDefinition entry)
		{
			try
			{
				return OriginalContextSymbol
					.GetMembers()
					.Where(m => m.Kind == SymbolKind.Property)
					.Cast<IPropertySymbol>()
					.First(m => m.Name == entry.Name); 
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to retrieve generic model's table property from: {OriginalContextSymbol.Name}. Prop: {entry.Name}:{entry.PropertyType}", e);
			}
		}
	}
}
