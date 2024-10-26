using System;
using System.Buffers;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
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
		public static ConcurrentDictionary<Type, object> RegisteredTypes { get; } = new();

		public static object SyncObj { get; } = new();

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
			if (RegisteredTypes.ContainsKey(typeof(TModelType)))
				return;

			// Failed to load game data. Error: System.InvalidOperationException: Failed to deserializer Type: FreecraftCore.AreaPOIEntry. Reason: System.TimeoutException: Timeout while inspecting metadata; this may indicate a deadlock. This can often be avoided by preparing necessary serializers during application initialization, rather than allowing multiple threads to perform the initial metadata inspection; please also see the LockContended event
			// For whatever reason afew times deserialization failed, maybe this lock will help?
			lock (SyncObj)
				RuntimeTypeModel.Default.Add(typeof(TModelType), true);
			//RuntimeTypeModel.Default.Add(typeof(GGDBFTable<TPrimaryKeyType, TModelType>), true);
			//RuntimeTypeModel.Default.Add(typeof(Dictionary<TPrimaryKeyType, TModelType>), true);

			RegisteredTypes.TryAdd(typeof(TModelType), null);
			//RegisteredTypes.Add(typeof(GGDBFTable<TPrimaryKeyType, TModelType>));
			//RegisteredTypes.Add(typeof(Dictionary<TPrimaryKeyType, TModelType>));

			if (typeof(TModelType).GetCustomAttribute<GeneratedCodeAttribute>() != null)
			{
				//TODO: This breaks if anyone has more than 100 props.
				lock(SyncObj)
					RuntimeTypeModel.Default
						.Add(typeof(TModelType).BaseType, true)
						.AddSubType(short.MaxValue, typeof(TModelType));

				RegisteredTypes.TryAdd(typeof(TModelType).BaseType, null);
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
				pool?.Return(pooledArray);
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
				// Don't use the LarayArrayPool because we'll end up
				// allocating a bunch of data which may not get reused since it's a custom
				// buffer and this is mostly being used in WebGL so that means we have
				// a bunch of large allocations that never go away.
				//pool = LargeArrayPool<byte>.Shared;
				pool = null;
				return new byte[size];
			}

			return pool.Rent(size);
		}
	}
}
