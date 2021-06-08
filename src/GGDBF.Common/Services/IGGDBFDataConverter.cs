using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// GGDBF's conversion service.
	/// Converts a table Type into a serializable <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/>.
	/// </summary>
	public interface IGGDBFDataConverter
	{
		/// <summary>
		/// Converts the internal data source's representation of the table to <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/>.
		/// </summary>
		/// <typeparam name="TPrimaryKeyType">The key type of the table.</typeparam>
		/// <typeparam name="TModelType">The model type of the table.</typeparam>
		/// <param name="keyResolutionFunction">Optional key resolution function that can retrieve the key from the model.</param>
		/// <returns>A serializable <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/> representation.</returns>
		GGDBFTable<TPrimaryKeyType, TModelType> Convert<TPrimaryKeyType, TModelType>(Func<TModelType, TPrimaryKeyType> keyResolutionFunction = null);
	}
}
