using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public interface IGGDBFWriteable
	{
		Task WriteAsync(IGGDBFDataWriter writer, CancellationToken token = default);
	}
}
