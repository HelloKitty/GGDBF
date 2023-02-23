using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class CompositeKeyTypeEmitter : BaseClassTypeEmitter
	{
		public INamedTypeSymbol ModelType { get; }

		public CompositeKeyTypeEmitter(string className, INamedTypeSymbol modelType, Accessibility classAccessibility = Accessibility.NotApplicable) 
			: base(className, classAccessibility)
		{
			ModelType = modelType;
		}

		public override void Emit(StringBuilder builder, CancellationToken token)
		{
			AppendGeneratedCodeAttribute(builder);
			builder.Append($"public record {ComputeContextTypeName()}(");

			string[] propertyNames = RetrieveKeyPropertyNames();

			for (var index = 0; index < propertyNames.Length; index++)
			{
				if (token.IsCancellationRequested)
					return;

				var entry = propertyNames[index];

				if (index < propertyNames.Length - 1)
					builder.Append($"{GetPropertyType(entry)} {entry}, ");
				else
					builder.Append($"{GetPropertyType(entry)} {entry}");
			}

			builder.Append(");");
		}

		private string[] RetrieveKeyPropertyNames()
		{
			try
			{
				return ModelType
					.GetAttributeExact<CompositeKeyHintAttribute>()
					.ConstructorArguments
					.First()
					.Values
					.Select(v => v.Value)
					.Cast<string>()
					.ToArray();
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to retrieve property names from: {ModelType.Name}'s {nameof(CompositeKeyHintAttribute)} Reason: {e.Message}", e);
			}
		}

		private string GetPropertyType(string entry)
		{
			//We need the model's type parameters, not the substituted ones from the DBContext.
			if (ModelType.IsGenericType)
				return ModelType
					.GetMembers()
					.Where(m => m.Kind == SymbolKind.Property)
					.Cast<IPropertySymbol>()
					.First(m => m.Name == entry)
					.OriginalDefinition
					.Type
					.ToFullName();

			//We do full name because we don't know the namespaces of members.
			return ModelType
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property)
				.Cast<IPropertySymbol>()
				.First(m => m.Name == entry)
				.Type
				.ToFullName();
		}

		private string ComputeContextTypeName()
		{
			return new TablePrimaryKeyParser().Parse(ModelType);
		}
	}
}
