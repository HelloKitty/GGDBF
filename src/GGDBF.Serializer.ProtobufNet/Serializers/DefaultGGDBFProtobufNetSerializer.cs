using System;
using System.Buffers;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Glader.Essentials;
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
				RuntimeTypeModel.Default.Add(typeof(TModelType), true);
				//RuntimeTypeModel.Default.Add(typeof(GGDBFTable<TPrimaryKeyType, TModelType>), true);
				//RuntimeTypeModel.Default.Add(typeof(Dictionary<TPrimaryKeyType, TModelType>), true);

				RegisteredTypes.Add(typeof(TModelType));
				//RegisteredTypes.Add(typeof(GGDBFTable<TPrimaryKeyType, TModelType>));
				//RegisteredTypes.Add(typeof(Dictionary<TPrimaryKeyType, TModelType>));

				if(typeof(TModelType).GetCustomAttribute<GeneratedCodeAttribute>() != null)
				{
					//TODO: This breaks if anyone has more than 100 props.
					RuntimeTypeModel.Default
						.Add(typeof(TModelType).BaseType, true)
						.AddSubType(short.MaxValue, typeof(TModelType));
				}
			}
		}

		/// <inheritdoc />
		public GGDBFTable<TPrimaryKeyType, TModelType> Deserialize<TPrimaryKeyType, TModelType>(byte[] bytes, int offset, int length)
		{
#if !PROTOBUF_OLD
			return Deserialize<TPrimaryKeyType, TModelType>(new ReadOnlySpan<byte>(bytes, offset, length));
#else
			using var ms = new MemoryStream(bytes, 0, length, false);
			return Serializer.Deserialize<GGDBFTable<TPrimaryKeyType, TModelType>>(ms);
#endif
		}

		public GGDBFTable<TPrimaryKeyType, TModelType> Deserialize<TPrimaryKeyType, TModelType>(ReadOnlySpan<byte> bytes)
		{
			RegisterTypeIfNotRegistered<TPrimaryKeyType, TModelType>();

			// Latest nuget has first-class span support
#if !PROTOBUF_OLD
			return Serializer.Deserialize<GGDBFTable<TPrimaryKeyType, TModelType>>(bytes);
#else
			// TODO: Hella memory allocs here
			var pooledArray = GetPooledBytes(bytes.Length, out var pool);
			try
			{
				// Best we can do is copy to pooled buffer
				// Not the fastest but better than ToBytes.
				bytes.CopyTo(pooledArray);
				return Deserialize<TPrimaryKeyType, TModelType>(pooledArray, 0, bytes.Length);
			}
			finally
			{
				pool.Return(pooledArray);
			}
#endif
		}

		private byte[] GetPooledBytes(int size, out ArrayPool<byte> pool)
		{
			if (size < (1024 * 1024))
			{
				pool = ArrayPool<byte>.Shared;
			}
			else
			{
				pool = LargeArrayPool<byte>.Shared;
			}

			return pool.Rent(size);
		}
	}
}
