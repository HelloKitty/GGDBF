using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class ForeignKeyHintAttribute : Attribute
	{
		/// <summary>
		/// The property name of the foreign-key value.
		/// </summary>
		public string KeyProperty { get; }

		/// <summary>
		/// The model-type of the navigation property.
		/// </summary>
		public Type NavigationPropertyType { get; }

		/// <summary>
		/// The name that the emitted navigation property should have.
		/// </summary>
		public string PropertyName { get; }

		public ForeignKeyHintAttribute(string keyProperty, Type navigationPropertyType, string propertyName)
		{
			KeyProperty = keyProperty;
			NavigationPropertyType = navigationPropertyType;
			PropertyName = propertyName;
		}
	}
}
