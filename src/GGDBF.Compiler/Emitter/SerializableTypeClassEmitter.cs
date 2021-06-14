using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GGDBF.Compiler;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class SerializableTypeClassEmitter : BaseClassTypeEmitter
	{
		private string ContextClassName { get; }

		private INamedTypeSymbol SerializableType { get; }

		public SerializableTypeClassEmitter(string className, string contextClassName, INamedTypeSymbol serializableType, Accessibility classAccessibility = Accessibility.NotApplicable)
			: base(className, classAccessibility)
		{
			if (string.IsNullOrWhiteSpace(contextClassName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(contextClassName));
			ContextClassName = contextClassName;
			SerializableType = serializableType ?? throw new ArgumentNullException(nameof(serializableType));
		}

		public override void Emit(StringBuilder builder)
		{
			//Class time!
			builder.Append($"[{nameof(GeneratedCodeAttribute)}(\"GGDBF\", \"{typeof(GGDBFTable<object, object>).Assembly.GetName().Version}\")]{Environment.NewLine}");
			if(ClassAccessibility == Accessibility.NotApplicable)
				builder.Append($"partial class {ClassName} : {ComputeTypeName(SerializableType)}, {nameof(IGGDBFSerializable)}{Environment.NewLine}{{");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial class {ClassName} : {ComputeTypeName(SerializableType)}, {nameof(IGGDBFSerializable)}{Environment.NewLine}{{");

			//TODO: Support ALL foreign key scenarios. Right now only traditional attribute-based PropId and Prop pair scenarios are supported
			//Find all foreign key references and generate
			//the required overrides for their navigation properties.
			foreach (var prop in SerializableType
				.GetMembers()
				.Where(m => m.HasAttributeLike<ForeignKeyAttribute>())
				.Cast<IPropertySymbol>())
			{
				IPropertySymbol navProperty = RetrieveNavigationPropertySymbol(prop);
				IPropertySymbol keyProperty = RetrieveNavigationKeyPropertySymbol(prop);

				builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
				builder.Append($"public override {navProperty.Type.Name} {navProperty.Name} {Environment.NewLine}{{ get => {ContextClassName}.Instance.{new TableNameParser().Parse(navProperty.Type)}[base.{keyProperty.Name}];{Environment.NewLine}");

				builder.Append($"}}{Environment.NewLine}");
			}

			int propCount = 1;
			foreach (var prop in EnumerateCollectionProperties())
			{
				ITypeSymbol collectionElementType = ComputeCollectionElementType(prop);
				string backingPropertyName = ComputeCollectionPropertyBackingFieldName(prop);

				//We must emit a serializable backing field for the collection property
				builder.Append($"[{nameof(DataMemberAttribute)}({nameof(DataMemberAttribute.Order)} = {propCount})]{Environment.NewLine}");
				builder.Append($"public {nameof(SerializableGGDBFCollection<int, object>)}<{new TablePrimaryKeyParser().Parse(collectionElementType)}, {collectionElementType.Name}> {backingPropertyName};{Environment.NewLine}{Environment.NewLine}");

				//The concept of returning the base property getter if the derived field is null for cases where we want to access the collection
				//during the process of serializing the real model type to the new model type. We need to access the collection and process it
				//for keys to build the serializable collection.
				//get => _ModelCollection != null ? _ModelCollection.Load(TestContext.Instance.Test4Datas) : base.ModelCollection;
				builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
				builder.Append($"public override {prop.Type.Name}<{collectionElementType.Name}> {prop.Name} {Environment.NewLine}{{ get => {backingPropertyName} != null ? {backingPropertyName}.{nameof(SerializableGGDBFCollection<int, object>.Load)}({ContextClassName}.Instance.{new TableNameParser().Parse(collectionElementType)}) : base.{prop.Name};{Environment.NewLine}");

				builder.Append($"}}{Environment.NewLine}");


				propCount++;
			}

			EmitSerializableInitializeMethod(builder);

			builder.Append($"}}");
		}

		private static ITypeSymbol ComputeCollectionElementType(IPropertySymbol prop)
		{
			return ((INamedTypeSymbol)prop.Type)
				.TypeArguments
				.First();
		}

		private static string ComputeCollectionPropertyBackingFieldName(IPropertySymbol prop)
		{
			return $"_{ prop.Name}";
		}

		private IEnumerable<IPropertySymbol> EnumerateCollectionProperties()
		{
			return SerializableType
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property)
				.Cast<IPropertySymbol>()
				.Where(p => p.IsICollectionType());
		}

		private void EmitSerializableInitializeMethod(StringBuilder builder)
		{
			builder.Append($"{Environment.NewLine}");
			builder.Append($"public void {nameof(IGGDBFSerializable.Initialize)}(){Environment.NewLine}{{");

			foreach (var prop in EnumerateCollectionProperties())
			{
				string fieldName = ComputeCollectionPropertyBackingFieldName(prop);
				ITypeSymbol collectionElementType = ComputeCollectionElementType(prop);
				string primaryKeyPropertyName = new TablePrimaryKeyParser().ParseProperty(collectionElementType).Name;

				builder.Append($"{fieldName} = {nameof(GGDBFHelpers)}.{nameof(GGDBFHelpers.CreateSerializableCollection)}(m => m.{primaryKeyPropertyName}, {prop.Name});");
			}

			builder.Append($"{Environment.NewLine}}}");
		}

		private IPropertySymbol RetrieveNavigationKeyPropertySymbol(IPropertySymbol prop)
		{
			//First check did they mark the KEY or the PROP
			//Assume all ref types are the nav property
			if(!prop.Type.IsReferenceType)
				return prop;

			return RetrieveLinkedForeignKeyProperty(prop);
		}

		private IPropertySymbol RetrieveNavigationPropertySymbol(IPropertySymbol prop)
		{
			//First check did they mark the KEY or the PROP
			//Assume all ref types are the nav property
			if(prop.Type.IsReferenceType)
				return prop;

			return RetrieveLinkedForeignKeyProperty(prop);
		}

		private IPropertySymbol RetrieveLinkedForeignKeyProperty(IPropertySymbol prop)
		{
			return SerializableType
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property)
				.Cast<IPropertySymbol>()
				.First(m => m.Kind == SymbolKind.Property && m.Name == (string) prop.GetAttributeLike<ForeignKeyAttribute>().ConstructorArguments.First().Value);
		}
	}
}
