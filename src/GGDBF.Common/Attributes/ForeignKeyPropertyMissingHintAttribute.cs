using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	//This should rarely if ever be used.
	/// <summary>
	/// Attribute marked on a model type that indicates a foreign key relationship exists
	/// to another model type but that the navigation property is missing but should be emitted.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class ForeignKeyPropertyMissingHintAttribute : Attribute
	{
		/// <summary>
		/// The model-type of the navigation property.
		/// </summary>
		public Type NavigationPropertyType { get; }

		/// <summary>
		/// The name that the emitted navigation property should have.
		/// </summary>
		public string PropertyName { get; }

		public ForeignKeyPropertyMissingHintAttribute(Type navigationPropertyType, string propertyName)
		{
			NavigationPropertyType = navigationPropertyType;
			PropertyName = propertyName;
		}
	}
}
