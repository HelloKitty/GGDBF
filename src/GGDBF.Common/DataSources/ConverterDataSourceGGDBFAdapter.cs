using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GGDBF.Generator;

namespace GGDBF
{
	/// <summary>
	/// <see cref="IGGDBFDataSource"/> adapter that will use a <see cref="IGGDBFDataConverter"/> to convert serializable types
	/// when reading from the datasource.
	/// WARNING: This only works for simple primary key cases. Complex/composite primary keys don't work unless the DataSource can deduce them.
	/// </summary>
	public sealed class ConverterDataSourceGGDBFAdapter : IGGDBFDataSource
	{
		/// <summary>
		/// Adapted datasource.
		/// </summary>
		private IGGDBFDataSource DataSource { get; }

		/// <summary>
		/// The model converter.
		/// </summary>
		private IGGDBFDataConverter Converter { get; }

		/// <summary>
		/// Creates a new converting datasource.
		/// </summary>
		/// <param name="dataSource">The datasource to adapt.</param>
		/// <param name="converter">The converter to use to convert between model to serializable model type.</param>
		public ConverterDataSourceGGDBFAdapter(IGGDBFDataSource dataSource, IGGDBFDataConverter converter)
		{
			DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
			Converter = converter ?? throw new ArgumentNullException(nameof(converter));
		}

		/// <inheritdoc />
		public async Task<IEnumerable<TModelType>> RetrieveAllAsync<TModelType>(CancellationToken token = default) 
			where TModelType : class
		{
			return await DataSource.RetrieveAllAsync<TModelType>(token);
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class
		{
			return await DataSource.RetrieveFullTableAsync(config, token);
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			var table = await DataSource.RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(config, token);

			return new GGDBFTable<TPrimaryKeyType, TModelType>()
			{
				Version = table.Version,
				TableName = table.TableName,
				TableData = table.TableData
					.ToDictionary(t => t.Key, t => (TModelType) Converter.Convert<TModelType, TSerializableModelType>(t.Value))
			};
		}
	}
}
