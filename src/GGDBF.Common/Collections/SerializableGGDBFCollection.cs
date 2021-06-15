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
		public TKeyType[] References { get; private set; } = Array.Empty<TKeyType>();

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

		public SerializableGGDBFCollection()
		{
			
		}

		public SerializableGGDBFCollection(TKeyType[] references)
		{
			References = references ?? throw new ArgumentNullException(nameof(references));
		}

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
		void ICollection<TModelType>.Add(TModelType item)
		{
			throw new NotSupportedException($"GGDBF does not support mutable collections.");
		}

		/// <inheritdoc />
		void ICollection<TModelType>.Clear()
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
		void ICollection<TModelType>.CopyTo(TModelType[] array, int arrayIndex)
		{
			throw new NotSupportedException($"GGDBF does not support mutable collections.");
		}

		/// <inheritdoc />
		bool ICollection<TModelType>.Remove(TModelType item)
		{
			throw new NotSupportedException($"GGDBF does not support mutable collections.");
		}

		/// <summary>
		/// Ensures the <see cref="ReferencedTable"/> property is initialized with the <see cref="table"/> parameter provided
		/// and returns the reference to the collection itself.
		/// </summary>
		/// <param name="table">The table to initialize the collection with.</param>
		/// <returns>This</returns>
		public SerializableGGDBFCollection<TKeyType, TModelType> Load(IReadOnlyDictionary<TKeyType, TModelType> table)
		{
			//It is not an optimization to check if already assigned.
			ReferencedTable = table;
			return this;
		}
	}
}
