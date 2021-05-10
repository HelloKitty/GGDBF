using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Attribute that can be applied to properties to indicate (or hint) that a property is the primary key for the data model.
	/// In cases where typical annotations are not or cannot be used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class DatabaseKeyHintAttribute : Attribute
	{
		/// <summary>
		/// Marks a property as the hinted primary key of the data model.
		/// </summary>
		public DatabaseKeyHintAttribute()
		{

		}
	}
}
