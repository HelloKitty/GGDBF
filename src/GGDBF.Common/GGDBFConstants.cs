using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	public static class GGDBFConstants
	{
		public const string FILE_EXTENSION_SUFFIX = "ggdbf";

		//TODO: Move this constant to Compiler section
		public static string[] DEFAULT_NAMESPACES = new string[]
		{
			"System",
			"System.Collections.Generic",
			"System.Threading.Tasks",
			"System.CodeDom.Compiler",
			"System.Runtime.Serialization",
			"GGDBF"
		};

		public const string INITIALIZE_METHOD_NAME = "Initialize";

		public const string CONTEXT_SINGLETON_PROPERTY_NAME = "Instance";
	}
}
