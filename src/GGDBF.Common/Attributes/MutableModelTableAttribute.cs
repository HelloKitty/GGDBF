using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Attribute that marks a model/table type as a mutable model.
	/// This means that uses of GGDBF can add or remove entries from the table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class MutableModelTableAttribute : Attribute
	{

	}
}
