using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;

namespace GGDBF
{
	public sealed class DefaultGGDBFProtobufNetSerializer : IGGDBFSerializer
	{
		public static HashSet<Type> RegisteredTypes { get; } = new HashSet<Type>();

		/// <inheritdoc />
		public byte[] Serialize<TPrimaryKeyType, TModelType>(GGDBFTable<TPrimaryKeyType, TModelType> table)
		{
			RegisterTypeIfNotRegistered<TPrimaryKeyType, TModelType>();

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

		private static void RegisterTypeIfNotRegistered<TPrimaryKeyType, TModelType>()
		{
			if (!RegisteredTypes.Contains(typeof(TModelType)))
			{
				RuntimeTypeModel.Default.Add(typeof(TModelType));
				RegisteredTypes.Add(typeof(TModelType));

				if(typeof(TModelType).GetCustomAttribute<GeneratedCodeAttribute>() != null)
				{
					//TODO: This breaks if anyone has more than 100 props.
					RuntimeTypeModel.Default
						.Add(typeof(TModelType).BaseType)
						.AddSubType(short.MaxValue, typeof(TModelType));
				}
			}
		}

		/// <inheritdoc />
		public GGDBFTable<TPrimaryKeyType, TModelType> Deserialize<TPrimaryKeyType, TModelType>(byte[] bytes, int offset, int length)
		{
			RegisterTypeIfNotRegistered<TPrimaryKeyType, TModelType>();

			return Serializer.Deserialize<GGDBFTable<TPrimaryKeyType, TModelType>>(new ReadOnlySpan<byte>(bytes, offset, length));
		}
	}
}
