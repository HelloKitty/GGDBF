using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		public abstract void Emit(StringBuilder builder, CancellationToken token);

		protected static string ComputeTypeName(PropertyDefinition prop)
		{
			return ComputeTypeName(prop.PropertyType);
		}

		protected static string ComputeTypeName(ITypeSymbol type)
		{
			return type.GetFriendlyName();
		}

		protected void CalculatePossibleTypeConstraints(INamedTypeSymbol type, StringBuilder builder)
		{
			if(type.IsGenericType)
			{
				var constraintString = new GenericTypeBuilder(type.TypeParameters.ToArray())
					.BuildConstraints();

				if(!String.IsNullOrWhiteSpace(constraintString))
					builder.Append($" {constraintString}");
			}
		}

		protected static void AppendGeneratedCodeAttribute(StringBuilder builder)
		{
			builder.Append($"[{nameof(GeneratedCodeAttribute)}(\"GGDBF\", \"{typeof(GGDBFTable<object, object>).Assembly.GetName().Version}\")]{Environment.NewLine}");
		}
	}
}
