using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Fasterflect;
using Microsoft.EntityFrameworkCore;

namespace GGDBF
{
	/// <summary>
	/// EntityFramework <see cref="DbContext"/>-base implementation of <see cref="IGGDBFDataSource"/>.
	/// </summary>
	/// <typeparam name="TContextType">The DB context type.</typeparam>
	public class EntityFrameworkGGDBFDataSource<TContextType> : IGGDBFDataSource
		where TContextType : DbContext
	{
		/// <summary>
		/// Internal data source.
		/// </summary>
		private TContextType Context { get; }

		public EntityFrameworkGGDBFDataSource(TContextType context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <inheritdoc />
		public async Task<IEnumerable<TModelType>> RetrieveAllAsync<TModelType>(CancellationToken token = default)
			where TModelType : class
		{
			return await Context
				.Set<TModelType>()
				.ToArrayAsync(token);
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default)
			where TModelType : class
		{
			IEnumerable<TModelType> models = await RetrieveAllAsync<TModelType>(token);

			if(config == null)
				throw new NotSupportedException($"TODO: Cannot support no config.");
			else if(config.KeyResolutionFunction == null)
			{
				//TODO: This only supports simple primary keys
				var keyName = Context.Model
					.FindEntityType(typeof(TModelType))
					.FindPrimaryKey()
					.Properties
					.Select(x => x.Name)
					.Single();

				config = new TableRetrievalConfig<TPrimaryKeyType, TModelType>(m => (TPrimaryKeyType)m.GetPropertyValue(keyName), config.TableNameOverride);
			}

			var map = new Dictionary<TPrimaryKeyType, TModelType>();
			var version = GGDBFHelpers.ConvertToVersion(typeof(GGDBFTable<TPrimaryKeyType, TModelType>).Assembly.GetName().Version);
			string name = string.IsNullOrWhiteSpace(config.TableNameOverride) ? typeof(TModelType).GetCustomAttribute<TableAttribute>(true).Name : config.TableNameOverride;

			foreach(var model in models)
				map[config.KeyResolutionFunction(model)] = model;

			return new GGDBFTable<TPrimaryKeyType, TModelType>()
			{
				TableData = map,
				Version = version,
				TableName = name
			};
		}

		/// <inheritdoc />
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveFullTableAsync<TPrimaryKeyType, TModelType, TSerializableModelType>(TableRetrievalConfig<TPrimaryKeyType, TModelType> config = null, CancellationToken token = default)
			where TModelType : class
			where TSerializableModelType : class, TModelType, IGGDBFSerializable
		{
			//EF Core doesn't support the concept of serializable subtypes.
			//So we can only return the actual model type.
			return await RetrieveFullTableAsync<TPrimaryKeyType, TModelType>(config, token);
		}
	}

	/// <summary>
	/// EntityFramework <see cref="DbContext"/>-base implementation of <see cref="IGGDBFDataSource"/>.
	/// </summary>
	public sealed class EntityFrameworkGGDBFDataSource : EntityFrameworkGGDBFDataSource<DbContext>
	{
		public EntityFrameworkGGDBFDataSource(DbContext context) 
			: base(context)
		{

		}
	}
}
