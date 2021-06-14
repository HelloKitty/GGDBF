using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Contract that can be implemented on types that required post-initialization for
	/// conversion/serialization.
	/// </summary>
	public interface IGGDBFSerializable
	{
		/// <summary>
		/// Initializes a model into a state where it can be serialized by GGDBF.
		/// </summary>
		void Initialize();
	}
}
