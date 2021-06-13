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
				builder.Append($"partial class {ClassName} : {SerializableType}{{");
			else
				builder.Append($"{ClassAccessibility.ToString().ToLower()} partial class {ClassName} : {SerializableType}{Environment.NewLine}{{");

			//TODO: Support ALL foreign key scenarios. Right now only traditional attribute-based PropId and Prop pair scenarios are supported
			//Find all foreign key references and generate
			//the required overrides for their navigation properties.
			int propCount = 1;
			foreach (var prop in SerializableType
				.GetMembers()
				.Where(m => m.HasAttributeLike<ForeignKeyAttribute>())
				.Cast<IPropertySymbol>())
			{
				IPropertySymbol navProperty = RetrieveNavigationPropertySymbol(prop);
				IPropertySymbol keyProperty = RetrieveNavigationKeyPropertySymbol(prop);

				builder.Append($"[{nameof(IgnoreDataMemberAttribute)}]{Environment.NewLine}");
				builder.Append($"public override {navProperty.Type.Name} {navProperty.Name} {Environment.NewLine}{{ get => {ContextClassName}.Instance.{new TableNameParser().Parse(navProperty.Type)}[base.{keyProperty.Name}];{Environment.NewLine}");

				//Not all properties setters should be overriden
				if (navProperty.SetMethod != null && navProperty.SetMethod.DeclaredAccessibility != Accessibility.Private)
					builder.Append($"{navProperty.SetMethod.DeclaredAccessibility.ToString().ToLower()} set => throw new InvalidOperationException(\"Cannot set readonly DB nav property.\");{Environment.NewLine}");

				builder.Append($"}}{Environment.NewLine}");
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
			return SerializableType
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property)
				.Cast<IPropertySymbol>()
				.First(m => m.Kind == SymbolKind.Property && m.Name == (string) prop.GetAttributeLike<ForeignKeyAttribute>().ConstructorArguments.First().Value);
		}
	}
}
