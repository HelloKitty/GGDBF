using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GGDBF
{
	/// <summary>
	/// EntityFramework <see cref="DbContext"/>-base implementation of <see cref="IGGDBFDataSource"/>.
	/// </summary>
	public sealed class EntityFrameworkGGDBFDataSource : IGGDBFDataSource
	{
		/// <summary>
		/// Internal data source.
		/// </summary>
		private DbContext Context { get; }

		public EntityFrameworkGGDBFDataSource(DbContext context)
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
		public async Task<GGDBFTable<TPrimaryKeyType, TModelType>> RetrieveTableAsync<TPrimaryKeyType, TModelType>(Func<TModelType, TPrimaryKeyType> keyResolutionFunction = null, CancellationToken token = default) 
			where TModelType : class
		{
			IEnumerable<TModelType> models = await RetrieveAllAsync<TModelType>(token);

			if (keyResolutionFunction == null)
				throw new NotSupportedException($"TODO: Cannot support no key resolution function.");

			var map = new Dictionary<TPrimaryKeyType, TModelType>();
			var version = GGDBFTable<TPrimaryKeyType, TModelType>.ConvertToVersion(typeof(GGDBFTable<TPrimaryKeyType, TModelType>).Assembly.GetName().Version);
			string name = typeof(TModelType).GetCustomAttribute<TableAttribute>(true).Name;

			foreach (var model in models)
				map[keyResolutionFunction(model)] = model;

			return new GGDBFTable<TPrimaryKeyType, TModelType>()
			{
				TableData = map,
				Version = version,
				TableName = name
			};
		}
	}
}
