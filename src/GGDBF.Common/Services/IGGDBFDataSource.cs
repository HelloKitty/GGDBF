using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	/// <summary>
	/// GGDBF's conversion service.
	/// Converts a table Type into a serializable <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/>.
	/// </summary>
	public interface IGGDBFDataSource
	{
		/// <summary>
		/// Retrieves all the models of the specified type from the data source.
		/// </summary>
		/// <typeparam name="TModelType">The model type.</typeparam>
		/// <param name="token">Cancel token.</param>
		/// <returns>Enumerable of all model types.</returns>
		Task<IEnumerable<TModelType>> RetrieveAllAsync<TModelType>(CancellationToken token = default)
			where TModelType : class;
	}
}
