using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class GenericTypeBuilder
	{
		private ITypeParameterSymbol[] TypeArgs { get; }

		public GenericTypeBuilder(ITypeParameterSymbol[] typeArgs)
		{
			TypeArgs = typeArgs ?? throw new ArgumentNullException(nameof(typeArgs));
		}

		public string Build(string className)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append($"{className}<");

			for(int i = 0; i < TypeArgs.Length; i++)
			{
				builder.Append(TypeArgs[i].Name);
				if(i < TypeArgs.Length - 1)
					builder.Append(", ");
			}

			builder.Append($">");
			return builder.ToString();
		}

		//See: https://stackoverflow.com/questions/8522332/why-is-some-ordering-enforced-in-generic-parameter-constraints
		public string BuildConstraints()
		{
			StringBuilder builder = new StringBuilder();
			for (var index = 0; index < TypeArgs.Length; index++)
			{
				var typeParameter = TypeArgs[index];
				List<string> constraints = new List<string>();
				if (typeParameter.HasReferenceTypeConstraint)
					constraints.Add($"class");
				else if (typeParameter.HasValueTypeConstraint && !typeParameter.HasUnmanagedTypeConstraint)
					constraints.Add("struct");
				else if (typeParameter.HasUnmanagedTypeConstraint)
					constraints.Add("unmanaged");

				//TODO: We need fully qualified names since we don't parse type constraints for namespace including
				//Now all types
				foreach (var type in typeParameter.ConstraintTypes)
					constraints.Add(type.ToFullName());

				if (!constraints.Any())
					continue;

				builder.Append($"where {typeParameter.Name} : ");

				for (int i = 0; i < constraints.Count; i++)
				{
					if (i < constraints.Count - 1)
						builder.Append($"{constraints[i]}, ");
					else
						builder.Append(constraints[i]);
				}
				
				if (index < TypeArgs.Length - 1)
					builder.Append($"{Environment.NewLine}");
			}

			return builder.ToString();
		}
	}
}
