using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public sealed class DefaultFileGGDBFDataWriter : IGGDBFDataWriter
	{
		private IGGDBFSerializer Serializer { get; }

		private string OutputPath { get; }

		private bool WriteToCurrentDirectory => OutputPath == null;

		public DefaultFileGGDBFDataWriter(IGGDBFSerializer serializer, string path = null)
		{
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			OutputPath = path;
		}

		/// <inheritdoc />
		public async Task WriteAsync<TPrimaryKeyType, TModelType>(GGDBFTable<TPrimaryKeyType, TModelType> table, CancellationToken token = default)
		{
			if (table == null) throw new ArgumentNullException(nameof(table));

			var bytes = Serializer.Serialize(table);

			//TODO: Any reason to support async?? Generator performance doesn't really matter tbh
			if (WriteToCurrentDirectory)
				File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), $"{table.TableName}.{GGDBFConstants.FILE_EXTENSION_SUFFIX}"), bytes);
			else
				File.WriteAllBytes(Path.Combine(OutputPath, $"{table.TableName}.{GGDBFConstants.FILE_EXTENSION_SUFFIX}"), bytes);
		}
	}
}
