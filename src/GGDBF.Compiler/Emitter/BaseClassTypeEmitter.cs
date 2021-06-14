using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public abstract class BaseClassTypeEmitter : ISourceEmitter
	{
		protected string ClassName { get; }

		protected Accessibility ClassAccessibility { get; }

		protected BaseClassTypeEmitter(string className, Accessibility classAccessibility = Accessibility.NotApplicable)
		{
			if(string.IsNullOrWhiteSpace(className)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(className));
			ClassName = className;
			ClassAccessibility = classAccessibility;
		}

		/// <inheritdoc />
		public abstract void Emit(StringBuilder builder);

		protected static string ComputeTypeName(PropertyDefinition prop)
		{
			return ComputeTypeName(prop.PropertyType);
		}

		protected static string ComputeTypeName(ITypeSymbol type)
		{
			if(type.ContainingNamespace != null)
				return $"{type.ToFullName().Replace($"{type.ContainingNamespace.Name}.", "")}";

			return $"{type.ToFullName()}";
		}
	}
}
