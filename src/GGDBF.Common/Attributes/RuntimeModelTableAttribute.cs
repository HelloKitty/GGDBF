using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Attribute that marks a model/table type as being runtime only.
	/// This means that no table loading code will be generated for the table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RuntimeModelTableAttribute : Attribute
	{

	}
}
