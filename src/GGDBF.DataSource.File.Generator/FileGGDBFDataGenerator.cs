using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public sealed class FileGGDBFDataGenerator<TGGDBFContextType>
		where TGGDBFContextType : class, IGGDBFContext, new()
	{
		/// <summary>
		/// The base URL for the requests.
		/// </summary>
		private string BasePath { get; }

		private IGGDBFSerializer SerializationStrategy { get; }

		public FileGGDBFDataGenerator(string basPath, 
			IGGDBFSerializer serializationStrategy)
		{
			if (string.IsNullOrWhiteSpace(basPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(basPath));
			BasePath = basPath;
			SerializationStrategy = serializationStrategy ?? throw new ArgumentNullException(nameof(serializationStrategy));
		}

		public async Task GenerateFilesAsync(TGGDBFContextType context, CancellationToken token = default)
		{
			// Walk and find all IGGDBFWriteable props and write
			foreach (var prop in typeof(TGGDBFContextType).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				// Now, we must check each actual prop value if it matches writable table
				if (prop.GetMethod == null)
					continue;

				var candidate = prop.GetValue(context);

				if (candidate == null)
					continue;

				if (candidate is not IGGDBFWriteable)
					continue;

				IGGDBFWriteable writable = candidate as IGGDBFWriteable;

				await writable.WriteAsync(new FileGGDBFDataWriter(SerializationStrategy, BasePath), token);
			}
		}
	}
}
