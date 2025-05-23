﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	/// <summary>
	/// Serializable Table model type for GGDBF that contains the version, name as well as the table contents
	/// as a <see cref="Dictionary{TKey,TValue}"/>.
	/// </summary>
	/// <typeparam name="TPrimaryKeyType">The key type for the table.</typeparam>
	/// <typeparam name="TModelType">The table model type.</typeparam>
	[DataContract]
	public sealed class GGDBFTable<TPrimaryKeyType, TModelType> : IGGDBFWriteable
	{
		/// <summary>
		/// GGDBF Version.
		/// (SemVer)
		/// </summary>
		[DataMember(Order = 1)]
		public int[] Version { get; init; } = new int[3];

		/// <summary>
		/// The name of the GGDBF table.
		/// </summary>
		[DataMember(Order = 2)]
		public string TableName { get; init; }

		/// <summary>
		/// Table contents.
		/// (Suggested to never modify this contents)
		/// </summary>
		[DataMember(Order = 3)]
		public IReadOnlyDictionary<TPrimaryKeyType, TModelType> TableData { get; init; }

		public GGDBFTable(string tableName, IReadOnlyDictionary<TPrimaryKeyType, TModelType> tableData)
		{
			TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
			TableData = tableData ?? throw new ArgumentNullException(nameof(tableData));
		}

		public GGDBFTable()
		{
			//protobuf might serialize this as null for empty tables.
			TableData = new Dictionary<TPrimaryKeyType, TModelType>();
		}

		/// <summary>
		/// Converts the <see cref="TableData"/> to a <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>A read-only <see cref="IReadOnlyDictionary{TKey,TValue}"/> version of <see cref="TableData"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlyDictionary<TPrimaryKeyType, TModelType> ToReadOnly()
		{
			return TableData;
		}

		/// <inheritdoc />
		public async Task WriteAsync(IGGDBFDataWriter writer, CancellationToken token = default)
		{
			await writer.WriteAsync(this, token);
		}
	}
}
