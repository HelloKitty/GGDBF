using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF.Compiler
{
	public interface ISourceEmitter
	{
		string Emit();

		void Emit(StringBuilder builder);
	}
}
