using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace GGDBF
{
	public sealed class DeferredConvertableGGDBFTableDictionary<TPrimaryKeyType, TModelType, TSerializableModelType> 
		: IReadOnlyDictionary<TPrimaryKeyType, TModelType>
		where TModelType : class
		where TSerializableModelType : class, TModelType, IGGDBFSerializable
	{
		private Lazy<GGDBFTable<TPrimaryKeyType, TModelType>> _ConvertedTable { get; }
		private IReadOnlyDictionary<TPrimaryKeyType, TModelType> ConvertedTable => _ConvertedTable.Value.TableData;

		private GGDBFTable<TPrimaryKeyType, TSerializableModelType> OriginalTable { get; set; }

		public DeferredConvertableGGDBFTableDictionary([NotNull] GGDBFTable<TPrimaryKeyType, TSerializableModelType> originalTable)
		{
			OriginalTable = originalTable ?? throw new ArgumentNullException(nameof(originalTable));

			_ConvertedTable = new Lazy<GGDBFTable<TPrimaryKeyType, TModelType>>(() =>
			{
				try
				{
					return OriginalTable.ConvertFrom<TPrimaryKeyType, TModelType, TSerializableModelType>();
				}
				finally
				{
					// Release data, we don't need it
					OriginalTable = null;
				}
			}, true);
		}

		public IEnumerator<KeyValuePair<TPrimaryKeyType, TModelType>> GetEnumerator()
		{
			return ConvertedTable.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) ConvertedTable).GetEnumerator();
		}

		public int Count => ConvertedTable.Count;

		public bool ContainsKey(TPrimaryKeyType key)
		{
			return ConvertedTable.ContainsKey(key);
		}

		public bool TryGetValue(TPrimaryKeyType key, out TModelType value)
		{
			return ConvertedTable.TryGetValue(key, out value);
		}

		public TModelType this[TPrimaryKeyType key] => ConvertedTable[key];

		public IEnumerable<TPrimaryKeyType> Keys => ConvertedTable.Keys;

		public IEnumerable<TModelType> Values => ConvertedTable.Values;
	}
}
