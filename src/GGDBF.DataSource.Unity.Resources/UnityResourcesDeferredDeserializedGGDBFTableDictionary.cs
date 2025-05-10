using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace GGDBF
{
	/// <summary>
	/// Deferred deserialized <see cref="IReadOnlyDictionary{TKey,TValue}"/>
	/// for use in Unity3D WebGL to defer expensive loading operations.
	/// </summary>
	/// <typeparam name="TKeyType"></typeparam>
	/// <typeparam name="TModelType"></typeparam>
	public sealed class UnityResourcesDeferredDeserializedGGDBFTableDictionary<TKeyType, TModelType>
		: IReadOnlyDictionary<TKeyType, TModelType>
	{
		private IGGDBFSerializer Serializer { get; }

		private Lazy<IReadOnlyDictionary<TKeyType, TModelType>> _DeserializedDictionary { get; }
		private IReadOnlyDictionary<TKeyType, TModelType> DeserializedDictionary => _DeserializedDictionary.Value;

		public UnityResourcesDeferredDeserializedGGDBFTableDictionary(string path,
			[NotNull] IGGDBFSerializer serializer)
		{
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

			_DeserializedDictionary = new Lazy<IReadOnlyDictionary<TKeyType, TModelType>>(() =>
			{
				try
				{
					// We're on WebGL for this so don't bother locking
					var asset = Resources.Load<TextAsset>(path);
					var table = Serializer.Deserialize<TKeyType, TModelType>(asset.bytes);

					return table.TableData;
				}
				finally
				{

				}
			}, true);
		}

		public IEnumerator<KeyValuePair<TKeyType, TModelType>> GetEnumerator()
		{
			return DeserializedDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) DeserializedDictionary).GetEnumerator();
		}

		public int Count => DeserializedDictionary.Count;

		public bool ContainsKey(TKeyType key)
		{
			return DeserializedDictionary.ContainsKey(key);
		}

		public bool TryGetValue(TKeyType key, out TModelType value)
		{
			return DeserializedDictionary.TryGetValue(key, out value);
		}

		public TModelType this[TKeyType key] => DeserializedDictionary[key];

		public IEnumerable<TKeyType> Keys => DeserializedDictionary.Keys;

		public IEnumerable<TModelType> Values => DeserializedDictionary.Values;
	}
}
