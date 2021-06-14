using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public interface IGGDBFDataWriter
	{
		Task WriteAsync<TPrimaryKeyType, TModelType>(GGDBFTable<TPrimaryKeyType, TModelType> table, CancellationToken token = default);
	}
}
