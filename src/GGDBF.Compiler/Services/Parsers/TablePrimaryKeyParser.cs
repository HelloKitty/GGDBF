using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;

namespace GGDBF
{
	public sealed class TablePrimaryKeyParser
	{
		public string Parse(INamedTypeSymbol type, bool useModelTypeParameters = false)
		{
			//First let's check for a CompositeKey attribute for types that have complex
			//key scenarios.
			if (type.HasAttributeExact<CompositeKeyHintAttribute>())
			{
				if (type.IsGenericType || type.IsUnboundGenericType)
					if (!useModelTypeParameters)
						return new GenericTypeBuilder(type.TypeParameters.ToArray()).Build($"{type.Name}Key");
					else
						return new GenericTypeBuilder(type.TypeParameters.ToArray()).Build($"{type.Name}Key", type.TypeArguments.ToArray());

				return $"{type.Name}Key";
			}

			return GetPrimaryKeyProperty(type).Type.GetFriendlyName();
		}

		public string ParseSimple(INamedTypeSymbol type)
		{
			//First let's check for a CompositeKey attribute for types that have complex
			//key scenarios.
			if(type.HasAttributeExact<CompositeKeyHintAttribute>())
				return $"{type.Name}Key";

			return GetPrimaryKeyProperty(type).Type.GetFriendlyName();
		}

		public string BuildCompositeKeyCreationExpression(INamedTypeSymbol type, string modelReference)
		{
			string[] keyProperties = type
				.GetAttributeExact<CompositeKeyHintAttribute>()
				.ConstructorArguments
				.First()
				.Values
				.Select(v => v.Value)
				.Cast<string>()
				.ToArray();

			StringBuilder builder = new StringBuilder();
			builder.Append($"new {Parse(type)}(");
			for(int i = 0; i < keyProperties.Length; i++)
			{
				builder.Append($"{modelReference}.{keyProperties[i]}");

				if(i < keyProperties.Length - 1)
					builder.Append(", ");
			}

			builder.Append(')');

			return builder.ToString();
		}

		public string BuildKeyResolutionLambda(INamedTypeSymbol type, string keyTypeNameOverride = null)
		{
			//First let's check for a CompositeKey attribute for types that have complex
			//key scenarios.
			if(type.HasAttributeExact<CompositeKeyHintAttribute>())
			{
				string[] keyProperties = type
					.GetAttributeExact<CompositeKeyHintAttribute>()
					.ConstructorArguments
					.First()
					.Values
					.Select(v => v.Value)
					.Cast<string>()
					.ToArray();

				if (keyTypeNameOverride == null)
					keyTypeNameOverride = Parse(type);

				StringBuilder builder = new StringBuilder();
				builder.Append($"m => new {keyTypeNameOverride}(");
				for (int i = 0; i < keyProperties.Length; i++)
				{
					builder.Append($"m.{keyProperties[i]}");

					if (i < keyProperties.Length - 1)
						builder.Append(", ");
				}

				builder.Append(')');

				return builder.ToString();
			}

			return $"m => m.{GetPrimaryKeyProperty(type).Name}";
		}

		private IPropertySymbol GetPrimaryKeyProperty(ITypeSymbol type)
		{
			foreach(var prop in type.GetMembers())
			{
				try
				{
					if(prop.Kind != SymbolKind.Property)
						continue;
					else if(prop.HasAttributeExact<KeyAttribute>() || prop.HasAttributeExact<DatabaseKeyHintAttribute>())
						return ((IPropertySymbol)prop);
				}
				catch(Exception e)
				{
					throw new InvalidOperationException($"Failed to parse PK from Type: {type.Name} on Property: {prop.Name} Reason: {e.Message} Stack: {e.StackTrace.Split('\n').LastOrDefault()}", e);
				}
			}

			throw new InvalidOperationException($"Failed to deduce PK from ModelType: {type.Name}:{type}");
		}

		public string BuildCompositeKeyCreationExpression(IPropertySymbol navProperty, string modelReference, INamedTypeSymbol referringType)
		{
			//TODO: We don't support all ForeignKey attribute setups
			//This overload will provide a key creation expression using the foreign key property on the
			//referring type.
			string[] keyProperties = ((string) navProperty
					.GetAttributeExact<ForeignKeyAttribute>()
					.ConstructorArguments.First()
					.Value)
				.Split(',')
				.Select(p => p.Replace(" ", ""))
				.ToArray();


			StringBuilder builder = new StringBuilder();
			builder.Append($"new {Parse((INamedTypeSymbol) navProperty.Type)}(");
			for(int i = 0; i < keyProperties.Length; i++)
			{
				builder.Append($"{modelReference}.{keyProperties[i]}");

				if(i < keyProperties.Length - 1)
					builder.Append(", ");
			}

			builder.Append(')');

			return builder.ToString();
		}
	}
}
