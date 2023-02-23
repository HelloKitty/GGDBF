using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GGDBF
{
	public interface ISourceEmitter
	{
		void Emit(StringBuilder builder, CancellationToken token);
	}
}
