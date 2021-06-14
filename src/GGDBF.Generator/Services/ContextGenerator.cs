using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GGDBF.Generator;

namespace GGDBF
{
	public sealed class ContextGenerator<TContextType>
		where TContextType : IGGDBFContext
	{
		private IGGDBFDataSource Source { get; }

		private IGGDBFDataWriter Writer { get; }

		public ContextGenerator(IGGDBFDataSource source, IGGDBFDataWriter writer)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			Writer = writer ?? throw new ArgumentNullException(nameof(writer));
		}

		public async Task Generate(CancellationToken token = default)
		{
			var sourceWriter = new DataSourceWriter(Source, new AutoMapperGGDBFDataConverter());

			//This is hacky but it's the way we can call the static Initialize method
			//It's ok to use hacky non-AOT friendly and slow reflection here because
			//this only runs offline
			await (Task)typeof(TContextType)
				.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public)
				.Invoke(null, new object[1] {sourceWriter});

			await sourceWriter.WriteAsync(Writer, token);
		}
	}
}
