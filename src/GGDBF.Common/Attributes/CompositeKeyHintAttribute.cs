using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class CompositeKeyHintAttribute : Attribute
	{
		public IEnumerable<string> PropertyNames { get; init; }

		public CompositeKeyHintAttribute(params string[] propertyNames)
		{
			PropertyNames = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
		}
	}
}
