using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	public interface IMutableGGDBFConcurrentDictionary<in TKey, TValue>
	{
		bool TryGetValue(TKey key, out TValue value);

		/// <summary>
		/// Determines whether the <see cref="IMutableGGDBFConcurrentDictionary{TKey,TValue}"/> contains the specified key.
		/// </summary>
		/// <param name="key">he key to locate in the <see cref="IMutableGGDBFConcurrentDictionary{TKey,TValue}"/></param>
		/// <returns>true if the <see cref="IMutableGGDBFConcurrentDictionary{TKey,TValue}"/> contains an element with the specified key; otherwise, false.</returns>
		bool ContainsKey(TKey key);

		/// <summary>
		/// Attempts to add the specified key and value to the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. It can be null.</param>
		/// <returns>true if the key/value pair was added to the dictionary successfully; otherwise, false.</returns>
		bool TryAdd(TKey key, TValue value);
	}

	public sealed class MutableGGDBFConcurrentDictionary<TKey, TValue> : IMutableGGDBFConcurrentDictionary<TKey, TValue>
	{
		private ConcurrentDictionary<TKey, TValue> Data { get; } = new();

		public MutableGGDBFConcurrentDictionary()
		{
			
		}

		/// <inheritdoc />
		public bool TryGetValue(TKey key, out TValue value)
		{
			return Data.TryGetValue(key, out value);
		}

		/// <inheritdoc />
		public bool ContainsKey(TKey key)
		{
			return Data.ContainsKey(key);
		}

		/// <inheritdoc />
		public bool TryAdd(TKey key, TValue value)
		{
			return Data.TryAdd(key, value);
		}
	}
}
