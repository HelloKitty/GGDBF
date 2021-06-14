using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Contract for serialization providers that can serialize <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/>.
	/// </summary>
	public interface IGGDBFSerializer
	{
		/// <summary>
		/// Serializes the provided <see cref="table"/> to bytes.
		/// </summary>
		/// <typeparam name="TPrimaryKeyType">The table's primary key.</typeparam>
		/// <typeparam name="TModelType">The table's model.</typeparam>
		/// <param name="table">The table to serialize.</param>
		/// <returns>Byte representation of the table.</returns>
		byte[] Serialize<TPrimaryKeyType, TModelType>(GGDBFTable<TPrimaryKeyType, TModelType> table);

		/// <summary>
		/// TODO: Doc
		/// </summary>
		/// <typeparam name="TPrimaryKeyType"></typeparam>
		/// <typeparam name="TModelType"></typeparam>
		/// <param name="bytes"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		GGDBFTable<TPrimaryKeyType, TModelType> Deserialize<TPrimaryKeyType, TModelType>(byte[] bytes, int offset, int length);
	}
}
