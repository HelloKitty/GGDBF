using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Serializable collection type that implements <see cref="ICollection{T}"/> to provide access to navigation property
	/// access for collection types.
	/// </summary>
	/// <typeparam name="TKeyType"></typeparam>
	/// <typeparam name="TModelType"></typeparam>
	[DataContract]
	public sealed class SerializableGGDBFCollection<TKeyType, TModelType> : ICollection<TModelType>
	{
		/// <summary>
		/// The serialized key values for the collection.
		/// </summary>
		[DataMember(Order = 1)]
		public TKeyType[] References { get; private set; }

		/// <summary>
		/// The table this collection references.
		/// Settable at runtime.
		/// </summary>
		[IgnoreDataMember]
		public IReadOnlyDictionary<TKeyType, TModelType> ReferencedTable { get; set; }

		/// <inheritdoc />
		[IgnoreDataMember]
		public int Count => References.Length;

		/// <inheritdoc />
		[IgnoreDataMember]
		public bool IsReadOnly => true;

		/// <inheritdoc />
		public IEnumerator<TModelType> GetEnumerator()
		{
			foreach (var reference in References)
				yield return ReferencedTable[reference];
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(TModelType item)
		{
			throw new NotSupportedException($"GGDBF does not support mutable collections.");
		}

		/// <inheritdoc />
		public void Clear()
		{
			throw new NotSupportedException($"GGDBF does not support mutable collections.");
		}

		/// <inheritdoc />
		public bool Contains(TModelType item)
		{
			foreach(var reference in References)
				 return Equals(ReferencedTable[reference], item);

			return false;
		}

		/// <inheritdoc />
		public void CopyTo(TModelType[] array, int arrayIndex)
		{
			throw new NotSupportedException($"GGDBF does not support mutable collections.");
		}

		/// <inheritdoc />
		public bool Remove(TModelType item)
		{
			throw new NotSupportedException($"GGDBF does not support mutable collections.");
		}
	}
}
