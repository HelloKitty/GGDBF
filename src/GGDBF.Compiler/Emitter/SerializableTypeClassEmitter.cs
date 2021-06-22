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
			//Class time! (or record lol)
			AppendGeneratedCodeAttribute(builder);
			builder.Append($"[{nameof(DataContractAttribute)}]{Environment.NewLine}");
			if(ClassAccessibility == Accessibility.NotApplicable)
				builder.Append($"partial {ComputeClassOrRecordKeyword()} {ComputeContextTypeName()} : {SerializableType.GetFriendlyName()}, {nameof(IGGDBFSerializable)}");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial {ComputeClassOrRecordKeyword()} {ComputeContextTypeName()} : {SerializableType.GetFriendlyName()}, {nameof(IGGDBFSerializable)}");

			CalculatePossibleTypeConstraints(OriginalContextSymbol, builder);

			builder.Append($"{Environment.NewLine}{{");

			//TODO: Support ALL foreign key scenarios. Right now only traditional attribute-based PropId and Prop pair scenarios are supported
			//Find all foreign key references and generate
			//the required overrides for their navigation properties.
			foreach (var prop in SerializableType
				.GetMembers()
				.Where(m => m.HasAttributeLike<ForeignKeyAttribute>() || m.HasAttributeExact<CompositeKeyHintAttribute>())
				.Cast<IPropertySymbol>())
			{
				//Special handling for composite key table references
				if (prop.Type.HasAttributeExact<CompositeKeyHintAttribute>() && prop.HasAttributeLike<CompositeKeyHintAttribute>())
				{
					//TODO: Hack to get the key name
					//TODO: Is it ok for open generics to use the type args??
					string keyTypeName = ComputeCompositeKeyTypeName(prop);
					string keyResolution = new TablePrimaryKeyParser().BuildCompositeKeyCreationExpression(prop, "base", keyTypeName);

					builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
					builder.Append($"public override {prop.Type.ToFullName()} {prop.Name} {Environment.NewLine}{{ get => {OriginalContextSymbol.GetFriendlyName()}.Instance.{new TableNameParser().Parse(prop.Type)}[{keyResolution}];{Environment.NewLine}");

					builder.Append($"}}{Environment.NewLine}");
				}
				else
				{
					IPropertySymbol navProperty = RetrieveNavigationPropertySymbol(prop);
					IPropertySymbol keyProperty = RetrieveNavigationKeyPropertySymbol(prop);

					builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
					builder.Append($"public override {navProperty.Type.ToFullName()} {navProperty.Name} {Environment.NewLine}{{ get => {OriginalContextSymbol.GetFriendlyName()}.Instance.{new TableNameParser().Parse(navProperty.Type)}[base.{keyProperty.Name}];{Environment.NewLine}");

					builder.Append($"}}{Environment.NewLine}");
				}
			}

			int propCount = 1;
			foreach (var prop in EnumerateForeignCollectionProperties())
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

			//This is for collections of owned types
			foreach(var prop in EnumerateOwnedTypeForeignCollectionProperties())
			{
				INamedTypeSymbol collectionElementType = (INamedTypeSymbol)ComputeCollectionElementType(prop);
				string backingPropertyName = ComputeCollectionPropertyBackingFieldName(prop);

				//We must emit a serializable backing field for the collection property
				builder.Append($"[{nameof(DataMemberAttribute)}({nameof(DataMemberAttribute.Order)} = {propCount})]{Environment.NewLine}");
				builder.Append($"public {ComputeOwnedTypeName(collectionElementType)}[] {backingPropertyName};{Environment.NewLine}{Environment.NewLine}");

				//Just return backing field or base prop getter if null (could be null too)
				builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
				builder.Append($"public override {prop.Type.Name}<{collectionElementType.GetFriendlyName()}> {prop.Name} {Environment.NewLine}{{ get => {backingPropertyName} != null ? {backingPropertyName} : base.{prop.Name};{Environment.NewLine}");

				builder.Append($"}}{Environment.NewLine}");

				propCount++;
			}

			if (!SerializableType.IsRecord)
				builder.Append($"public {ClassName}() {{ }}{Environment.NewLine}");

			EmitSerializableInitializeMethod(builder);

			builder.Append($"}}");

			//For all owned types we should maybe generate keys
			if (SerializableType.HasOwnedTypePropertyWithForeignKey())
			{
				foreach (var prop in SerializableType
					.GetMembers()
					.Where(m => m.IsVirtual && m.Kind == SymbolKind.Property)
					.Cast<IPropertySymbol>()
					.Where(p => p.HasAttributeLike<OwnedTypeHintAttribute>() && p.IsOwnedPropertyWithForeignKey()))
				{
					//TODO: If multiple owned types with generic type parameters are used in the same table then this won't work.
					INamedTypeSymbol ownedType = (INamedTypeSymbol) (prop.IsICollectionType() ? ((INamedTypeSymbol)prop.Type).TypeArguments.First() : prop.Type);
					SerializableTypeClassEmitter emitter = new SerializableTypeClassEmitter(ComputeOwnedTypeName(ownedType), ownedType, OriginalContextSymbol, ClassAccessibility);

					builder.Append($"{Environment.NewLine}{Environment.NewLine}");
					emitter.Emit(builder);
				}
			}
		}

		private string ComputeOwnedTypeName(INamedTypeSymbol ownedType)
		{
			return $"{ComputeContextTypeName()}_{ownedType.Name}";
		}

		private string ComputeClassOrRecordKeyword()
		{
			return SerializableType.IsRecord ? "record" : "class";
		}

		private static string ComputeCompositeKeyTypeName(IPropertySymbol prop)
		{
			if (((INamedTypeSymbol)prop.Type).IsGenericType)
				return new GenericTypeBuilder(((INamedTypeSymbol)prop.Type).TypeParameters.ToArray()).Build($"{prop.Type.Name}Key", ((INamedTypeSymbol)prop.Type).TypeArguments.ToArray());
			return new TablePrimaryKeyParser().Parse((INamedTypeSymbol) prop.Type);
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

		private IEnumerable<IPropertySymbol> EnumerateForeignCollectionProperties()
		{
			return SerializableType
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property && m.IsVirtual)
				.Cast<IPropertySymbol>()
				.Where(p => p.IsICollectionType() && ComputeCollectionElementType(p).HasAttributeLike<TableAttribute>()); //important to ignore non-table types (probably complex/owned types)
		}

		private IEnumerable<IPropertySymbol> EnumerateOwnedTypeForeignCollectionProperties()
		{
			return SerializableType
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property && m.IsVirtual)
				.Cast<IPropertySymbol>()
				.Where(p => p.IsICollectionType() && (p.HasAttributeExact<OwnedTypeHintAttribute>() || p.Type.HasAttributeExact<OwnedTypeHintAttribute>())); //important to ignore non-table types (probably complex/owned types)
		}

		private void EmitSerializableInitializeMethod(StringBuilder builder)
		{
			builder.Append($"{Environment.NewLine}");
			builder.Append($"public void {nameof(IGGDBFSerializable.Initialize)}({nameof(IGGDBFDataConverter)} converter){Environment.NewLine}{{{Environment.NewLine}");

			foreach (var prop in EnumerateForeignCollectionProperties())
			{
				string fieldName = ComputeCollectionPropertyBackingFieldName(prop);
				INamedTypeSymbol collectionElementType = (INamedTypeSymbol) ComputeCollectionElementType(prop);

				//TODO: Hack to get the key name
				//TODO: Is it ok for open generics to use the type args??
				string keyTypeName = new GenericTypeBuilder(collectionElementType.TypeParameters.ToArray()).Build($"{collectionElementType.Name}Key", collectionElementType.TypeArguments.ToArray());
				string keyResolutionLambda = new TablePrimaryKeyParser().BuildKeyResolutionLambda(collectionElementType, keyTypeName);

				builder.Append($"{fieldName} = {nameof(GGDBFHelpers)}.{nameof(GGDBFHelpers.CreateSerializableCollection)}({keyResolutionLambda}, {prop.Name});{Environment.NewLine}");
			}

			foreach (var prop in EnumerateOwnedTypeForeignCollectionProperties())
			{
				string fieldName = ComputeCollectionPropertyBackingFieldName(prop);
				INamedTypeSymbol collectionElementType = (INamedTypeSymbol)ComputeCollectionElementType(prop);

				builder.Append($"{fieldName} = converter.{nameof(IGGDBFDataConverter.Convert)}<{collectionElementType.ToFullName()}, {ComputeOwnedTypeName(collectionElementType)}>({prop.Name});{Environment.NewLine}");
			}

			builder.Append($"}}");
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
