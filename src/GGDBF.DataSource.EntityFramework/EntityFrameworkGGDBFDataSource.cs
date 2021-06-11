using System;
using System.Collections.Generic;
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
	}
}
