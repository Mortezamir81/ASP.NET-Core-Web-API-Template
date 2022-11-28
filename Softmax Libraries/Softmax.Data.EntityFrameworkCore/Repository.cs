using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Dtat.Data.EntityFrameworkCore
{
	public abstract class Repository<TEntity> :
		IRepository<TEntity> where TEntity : class
	{
		public Repository(DbContext databaseContext) : base()
		{
			DatabaseContext =
				databaseContext ?? throw new ArgumentNullException(paramName: nameof(databaseContext));

			DbSet =
				DatabaseContext.Set<TEntity>();
		}

		protected DbContext DatabaseContext { get; }

		protected DbSet<TEntity> DbSet { get; }


		public async Task<IEnumerable<TEntity>> 
			GetAllAsync(CancellationToken cancellationToken = default)
		{
			var result =
				await DbSet.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

			return result;
		}

		public async Task<TEntity> GetByIdAsync
			(Guid? id, CancellationToken cancellationToken = default)
		{
			if (id == null)
			{
				throw new ArgumentNullException(paramName: nameof(id));
			}

			var entity =
				await DbSet.FindAsync(keyValues: id);

			if (entity != null)
				DatabaseContext.Entry(entity).State = EntityState.Detached;

			return entity;
		}

		public async Task<TEntity> GetByIdWithTrackingAsync
			(Guid? id, CancellationToken cancellationToken = default)
		{
			if (id == null)
			{
				throw new ArgumentNullException(paramName: nameof(id));
			}

			var entity =
				await DbSet.FindAsync(keyValues: id);

			return entity;
		}

		public Task<IEnumerable<TEntity>>
			GetSomeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}


		public async Task AddAsync
			(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(paramName: nameof(entity));
			}

			var result =
				await DbSet.AddAsync
					(entity: entity, cancellationToken: cancellationToken);
		}

		public async Task AddRangeAsync
			(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		{
			if (entities == null)
			{
				throw new ArgumentNullException(paramName: nameof(entities));
			}

			 await DbSet.AddRangeAsync
				(entities: entities, cancellationToken: cancellationToken);
		}

		public async Task RemoveAsync
			(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(paramName: nameof(entity));
			}

			await Task.Run(() =>
			{
				var result =
					DbSet.Remove(entity: entity);

			}, cancellationToken: cancellationToken);
		}

		public async Task<bool> RemoveByIdAsync
			(Guid? id, CancellationToken cancellationToken = default)
		{
			TEntity entity =
				await GetByIdAsync
					(id: id, cancellationToken: cancellationToken);

			if (entity == null)
			{
				return false;
			}

			 await RemoveAsync
				(entity: entity, cancellationToken: cancellationToken);

			return true;
		}

		public async Task RemoveRangeAsync
			(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
		{
			if (entities == null)
			{
				throw new ArgumentNullException(paramName: nameof(entities));
			}

			await Task.Run(() =>
			{
				DatabaseContext.RemoveRange(entities: entities);

			}, cancellationToken: cancellationToken);
		}

		public async Task UpdateAsync
			(TEntity entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException(paramName: nameof(entity));
			}

			await Task.Run(() =>
			{
				DbSet.Update(entity);

			}, cancellationToken: cancellationToken);
		}
	}
}
