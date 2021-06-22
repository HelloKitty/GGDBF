using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Hints to GGDBF that this is an OwnedType but not a Table type
	/// and should generate foreign key properties if required.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class OwnedTypeHintAttribute : Attribute
	{

	}
}
