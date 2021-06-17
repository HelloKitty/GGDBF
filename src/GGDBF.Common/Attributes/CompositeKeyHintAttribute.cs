using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Attribute that can be marked on Types or Navigation Properties indicating that the keys involved
	/// are composite keys.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class CompositeKeyHintAttribute : Attribute
	{
		/// <summary>
		/// The name of the properties that represent components of the composite key.
		/// </summary>
		public IEnumerable<string> PropertyNames { get; init; }

		public CompositeKeyHintAttribute(params string[] propertyNames)
		{
			PropertyNames = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
		}
	}
}
