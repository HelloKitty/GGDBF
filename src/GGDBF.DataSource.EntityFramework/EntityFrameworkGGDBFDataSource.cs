﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Fasterflect;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;

namespace GGDBF
{
	/// <summary>
	/// EntityFramework <see cref="DbContext"/>-base implementation of <see cref="IGGDBFDataSource"/>.
	/// </summary>
	/// <typeparam name="TContextType">The DB context type.</typeparam>
	public class EntityFrameworkGGDBFDataSource<TContextType> : IGGDBFDataSource
		where TContextType : DbContext
	{
		// TODO: Unfortunately because we share this DB Context (and we know that some things will try to call this multi-threaded we need to lock
		// One day, when we can inject a DBContext factory in EF Core 5 we won't ave to do this. But this is the only way, hurts perf but it's the only solution at the moment
		private AsyncLock ContextLock { get; } = new();

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
			using(await ContextLock.LockAsync(token))
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
				using (await ContextLock.LockAsync(token))
				{
					var keyName = Context.Model
						.FindEntityType(typeof(TModelType))
						.FindPrimaryKey()
						.Properties
						.Select(x => x.Name)
						.Single();

					config = new TableRetrievalConfig<TPrimaryKeyType, TModelType>(m => (TPrimaryKeyType)m.GetPropertyValue(keyName), config.TableNameOverride);
				}
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

		/// <inheritdoc />
		public async Task ReloadAsync(CancellationToken token = default)
		{
			//Not quite a good implementation but this is the closest logical
			//thing to a Reload/Refresh for a DBContext
			//See: https://stackoverflow.com/questions/20270599/entity-framework-refresh-context
			using(await ContextLock.LockAsync(token))
				foreach(var entity in Context.ChangeTracker.Entries())
					await entity.ReloadAsync(token);
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
