using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Attribute marked on a model type that indicates a foreign key relationship exists
	/// between the two properties.
	/// (Ex. Useful for when foreign key relationships are defined in code rather than annotations)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ForeignKeyHintAttribute : Attribute
	{
		public string KeyProperty { get; }

		public string NavigationProperty { get; }

		public ForeignKeyHintAttribute(string keyProperty, string navigationProperty)
		{
			if (string.IsNullOrWhiteSpace(keyProperty)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(keyProperty));
			if (string.IsNullOrWhiteSpace(navigationProperty)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(navigationProperty));

			KeyProperty = keyProperty;
			NavigationProperty = navigationProperty;
		}
	}
}
