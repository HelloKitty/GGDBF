using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGDBF
{
	public static class GGDBFTableExtensions
	{
		/// <summary>
		/// Converts a <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/> to another <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/>
		/// with a more derived typed value type.
		/// </summary>
		/// <typeparam name="TPrimaryKeyType">Primary table key.</typeparam>
		/// <typeparam name="TModelType">Model type.</typeparam>
		/// <typeparam name="TSerializableModelType">The derived model type.</typeparam>
		/// <param name="table">The table to convert.</param>
		/// <returns></returns>
		public static GGDBFTable<TPrimaryKeyType, TSerializableModelType> ConvertTo<TPrimaryKeyType, TModelType, TSerializableModelType>(this GGDBFTable<TPrimaryKeyType, TModelType> table)
			where TSerializableModelType : TModelType
		{
			if (table == null) throw new ArgumentNullException(nameof(table));

			return new GGDBFTable<TPrimaryKeyType, TSerializableModelType>()
			{
				Version = table.Version,
				TableName = table.TableName,
				TableData = table.TableData.ToDictionary(kvp => kvp.Key, kvp => (TSerializableModelType) kvp.Value)
			};
		}

		/// <summary>
		/// Converts a <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/> to another <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/>
		/// with a less derived type.
		/// </summary>
		/// <typeparam name="TPrimaryKeyType">Primary table key.</typeparam>
		/// <typeparam name="TModelType">Model type.</typeparam>
		/// <typeparam name="TSerializableModelType">The derived model type.</typeparam>
		/// <param name="table">The table to convert.</param>
		/// <returns></returns>
		public static GGDBFTable<TPrimaryKeyType, TModelType> ConvertFrom<TPrimaryKeyType, TModelType, TSerializableModelType>(this GGDBFTable<TPrimaryKeyType, TSerializableModelType> table)
			where TSerializableModelType : TModelType
		{
			if(table == null) throw new ArgumentNullException(nameof(table));

			return new GGDBFTable<TPrimaryKeyType, TModelType>()
			{
				Version = table.Version,
				TableName = table.TableName,
				TableData = table.TableData.ToDictionary(kvp => kvp.Key, kvp => (TModelType)kvp.Value)
			};
		}
	}
}
