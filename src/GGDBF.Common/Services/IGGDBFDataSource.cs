using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GGDBF
{
	public record TableRetrievalConfig<TPrimaryKeyType, TModelType>(
		Func<TModelType, TPrimaryKeyType> KeyResolutionFunction = null,
		string TableNameOverride = null);

	public record NameOverrideTableRetrievalConfig<TPrimaryKeyType, TModelType>(string TableNameOverride) 
		: TableRetrievalConfig<TPrimaryKeyType, TModelType>(null, TableNameOverride);

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

		/// <summary>
		/// Converts the internal data source's representation of the table to <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/>.
		/// </summary>
		/// <typeparam name="TPrimaryKeyType">The key type of the table.</typeparam>
		/// <typeparam name="TModelType">The model type of the table.</typeparam>
		/// <param name="config">Configuration for retrieving the table.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns>A serializable <see cref="GGDBFTable{TPrimaryKeyType,TModelType}"/> representation.</returns>
		Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default)
			where TModelType : class;
	}
}
