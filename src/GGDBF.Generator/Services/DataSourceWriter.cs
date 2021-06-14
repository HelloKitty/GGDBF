using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GGDBF.Generator;

namespace GGDBF
{
	public sealed class DataSourceWriter : IGGDBFDataSource, IGGDBFWriteable
	{
		private IGGDBFDataSource DataSource { get; }

		private IGGDBFDataConverter Converter { get; }

		private List<IGGDBFWriteable> Writeables { get; } = new();

		public DataSourceWriter(IGGDBFDataSource dataSource, IGGDBFDataConverter converter)
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
			var table = await DataSource.RetrieveFullTableAsync(config, token);
			Writeables.Add(table);
			return table;
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default) 
			where TModelType : class 
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			//We run the converter here after the data is retrieved
			//This is so that the serializable type is actually the one being returned for serialization
			GGDBFTable<TPrimaryKeyType, TModelType> table = await DataSource.RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(config, token);

			//We add the serializable version to the writeables
			//otherwise we'll serialize the type that doesn't support navigation properties
			Writeables.Add(new GGDBFTable<TPrimaryKeyType, TSerializableModelType>()
			{
				TableData = table.TableData
					.Select(kvp => new KeyValuePair<TPrimaryKeyType, TSerializableModelType>(kvp.Key, Converter.Convert<TModelType, TSerializableModelType>(kvp.Value)))
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
				Version = table.Version,
				TableName = table.TableName
			});

			return table;
		}

		public async Task WriteAsync(IGGDBFDataWriter writer, CancellationToken token = default)
		{
			if (writer == null) throw new ArgumentNullException(nameof(writer));

			foreach (var writeable in Writeables)
				await writeable.WriteAsync(writer, token);
		}
	}
}
