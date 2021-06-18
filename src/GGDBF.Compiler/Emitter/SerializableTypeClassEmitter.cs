using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class SerializableTypeClassEmitter : BaseClassTypeEmitter
	{
		private INamedTypeSymbol SerializableType { get; }

		private INamedTypeSymbol OriginalContextSymbol { get; }

		public SerializableTypeClassEmitter(string className, INamedTypeSymbol serializableType, INamedTypeSymbol originalContextSymbol, Accessibility classAccessibility = Accessibility.NotApplicable)
			: base(className, classAccessibility)
		{
			SerializableType = serializableType ?? throw new ArgumentNullException(nameof(serializableType));
			OriginalContextSymbol = originalContextSymbol ?? throw new ArgumentNullException(nameof(originalContextSymbol));
		}

		public override void Emit(StringBuilder builder)
		{
			//Class time!
			AppendGeneratedCodeAttribute(builder);
			builder.Append($"[{nameof(DataContractAttribute)}]{Environment.NewLine}");
			if(ClassAccessibility == Accessibility.NotApplicable)
				builder.Append($"partial class {ComputeContextTypeName()} : {SerializableType.GetFriendlyName()}, {nameof(IGGDBFSerializable)}");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial class {ComputeContextTypeName()} : {SerializableType.GetFriendlyName()}, {nameof(IGGDBFSerializable)}");

			CalculatePossibleTypeConstraints(OriginalContextSymbol, builder);

			builder.Append($"{Environment.NewLine}{{");

			//TODO: Support ALL foreign key scenarios. Right now only traditional attribute-based PropId and Prop pair scenarios are supported
			//Find all foreign key references and generate
			//the required overrides for their navigation properties.
			foreach (var prop in SerializableType
				.GetMembers()
				.Where(m => m.HasAttributeLike<ForeignKeyAttribute>())
				.Cast<IPropertySymbol>())
			{
				IPropertySymbol navProperty = RetrieveNavigationPropertySymbol(prop);

				//Special handling for composite key table references
				if (navProperty.Type.HasAttributeExact<CompositeKeyHintAttribute>())
				{
					string keyResolution = new TablePrimaryKeyParser().BuildCompositeKeyCreationExpression(navProperty, "base", SerializableType);

					builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
					builder.Append($"public override {navProperty.Type.Name} {navProperty.Name} {Environment.NewLine}{{ get => {OriginalContextSymbol.GetFriendlyName()}.Instance.{new TableNameParser().Parse(navProperty.Type)}[{keyResolution}];{Environment.NewLine}");

					builder.Append($"}}{Environment.NewLine}");
				}
				else
				{
					IPropertySymbol keyProperty = RetrieveNavigationKeyPropertySymbol(prop);

					builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
					builder.Append($"public override {navProperty.Type.Name} {navProperty.Name} {Environment.NewLine}{{ get => {OriginalContextSymbol.GetFriendlyName()}.Instance.{new TableNameParser().Parse(navProperty.Type)}[base.{keyProperty.Name}];{Environment.NewLine}");

					builder.Append($"}}{Environment.NewLine}");
				}
			}

			int propCount = 1;
			foreach (var prop in EnumerateCollectionProperties())
			{
				INamedTypeSymbol collectionElementType = (INamedTypeSymbol) ComputeCollectionElementType(prop);
				string backingPropertyName = ComputeCollectionPropertyBackingFieldName(prop);

				//We must emit a serializable backing field for the collection property
				builder.Append($"[{nameof(DataMemberAttribute)}({nameof(DataMemberAttribute.Order)} = {propCount})]{Environment.NewLine}");
				builder.Append($"public {nameof(SerializableGGDBFCollection<int, object>)}<{new TablePrimaryKeyParser().Parse(collectionElementType, true)}, {collectionElementType.GetFriendlyName()}> {backingPropertyName};{Environment.NewLine}{Environment.NewLine}");

				//The concept of returning the base property getter if the derived field is null for cases where we want to access the collection
				//during the process of serializing the real model type to the new model type. We need to access the collection and process it
				//for keys to build the serializable collection.
				//get => _ModelCollection != null ? _ModelCollection.Load(TestContext.Instance.Test4Datas) : base.ModelCollection;
				builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
				builder.Append($"public override {prop.Type.Name}<{collectionElementType.GetFriendlyName()}> {prop.Name} {Environment.NewLine}{{ get => {backingPropertyName} != null ? {backingPropertyName}.{nameof(SerializableGGDBFCollection<int, object>.Load)}({OriginalContextSymbol.GetFriendlyName()}.Instance.{new TableNameParser().Parse(collectionElementType)}) : base.{prop.Name};{Environment.NewLine}");

				builder.Append($"}}{Environment.NewLine}");


				propCount++;
			}

			builder.Append($"public {ClassName}() {{ }}{Environment.NewLine}");

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
			return $"_Serialized{prop.Name}";
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
				INamedTypeSymbol collectionElementType = (INamedTypeSymbol) ComputeCollectionElementType(prop);

				//TODO: Hack to get the key name
				//TODO: Is it ok for open generics to use the type args??
				string keyTypeName = new GenericTypeBuilder(collectionElementType.TypeParameters.ToArray()).Build($"{collectionElementType.Name}Key", collectionElementType.TypeArguments.ToArray());
				string keyResolutionLambda = new TablePrimaryKeyParser().BuildKeyResolutionLambda(collectionElementType, keyTypeName);

				builder.Append($"{fieldName} = {nameof(GGDBFHelpers)}.{nameof(GGDBFHelpers.CreateSerializableCollection)}({keyResolutionLambda}, {prop.Name});");
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
			try
			{
				return SerializableType
					.GetMembers()
					.Where(m => m.Kind == SymbolKind.Property)
					.Cast<IPropertySymbol>()
					.First(m => m.Kind == SymbolKind.Property && m.Name == (string)prop.GetAttributeLike<ForeignKeyAttribute>().ConstructorArguments.First().Value);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to retrieve foreign key property for Type: {SerializableType.Name} Prop: {prop.Name}", e);
			}
		}

		private string ComputeContextTypeName()
		{
			if(!OriginalContextSymbol.IsGenericType)
				return ClassName;
			else
			{
				return new GenericTypeBuilder(OriginalContextSymbol.TypeParameters.ToArray())
					.Build(ClassName);
			}
		}
	}
}
