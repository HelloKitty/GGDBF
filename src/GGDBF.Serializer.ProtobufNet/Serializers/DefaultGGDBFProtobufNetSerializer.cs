using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

namespace GGDBF
{
	public sealed class DefaultGGDBFProtobufNetSerializer : IGGDBFSerializer
	{
		/// <inheritdoc />
		public byte[] Serialize<TPrimaryKeyType, TModelType>(GGDBFTable<TPrimaryKeyType, TModelType> table)
		{
			//This allocates like HELL and is not efficient but this process is
			//offline for running time of the applications (only generation)
			//so ignore perf.
			using (MemoryStream ms = new MemoryStream())
			{
				Serializer.Serialize(ms, table);
				ms.Position = 0;
				return ms.ToArray();
			}
		}

		/// <inheritdoc />
		public GGDBFTable<TPrimaryKeyType, TModelType> Deserialize<TPrimaryKeyType, TModelType>(byte[] bytes)
		{
			return Serializer.Deserialize<GGDBFTable<TPrimaryKeyType, TModelType>>(new ReadOnlyMemory<byte>(bytes));
		}
	}
}
