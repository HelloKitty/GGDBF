using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Attribute that enables pretty C#-IDE like auto-formatting for generated source.
	/// This is useful for debugging and inspecting the generated source code.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EnableDebugFormattingAttribute : Attribute
	{

	}
}
