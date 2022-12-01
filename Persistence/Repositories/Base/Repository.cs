namespace Persistence.Repositories.Base;

public class RepositoryBase<TEntity> : 
	IRepositoryBase<TEntity> where TEntity : class
{
	#region Constractor
	public RepositoryBase(DatabaseContext dbContext)
	{
		DatabaseContext = dbContext;

		Entities = DatabaseContext.Set<TEntity>();
	}
	#endregion /Constractor

	#region Properties
	protected readonly DatabaseContext DatabaseContext;

	public DbSet<TEntity> Entities { get; }

	public virtual IQueryable<TEntity> Table => Entities;

	public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();
	#endregion /Properties

	#region Add
	public virtual void Add(TEntity entity, bool saveNow = true)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		Entities.Add(entity);

		if (saveNow)
			DatabaseContext.SaveChanges();
	}


	public virtual async Task AddAsync
		(TEntity entity, CancellationToken cancellationToken = default, bool saveNow = true)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);

		if (saveNow)
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}


	public virtual void AddRange
		(IEnumerable<TEntity> entities, bool saveNow = true)
	{
		Assert.NotNull(obj: entities, name: nameof(entities));

		Entities.AddRange(entities);

		if (saveNow)
			DatabaseContext.SaveChanges();
	}


	public virtual async Task AddRangeAsync
		(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool saveNow = true)
	{
		Assert.NotNull(obj: entities, name: nameof(entities));

		await Entities.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

		if (saveNow)
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion /Add

	#region Get
	public virtual TEntity? GetById(params object[] ids)
	{
		return Entities!.Find(ids);
	}


	public virtual async Task<TEntity?> GetByIdAsync
		(CancellationToken cancellationToken = default, params object[] ids)
	{
		return await Entities!.FindAsync(ids, cancellationToken);
	}


	public async Task<IEnumerable<TEntity>> GetAllAsync
		(bool asNoTracking = false, CancellationToken cancellationToken = default)
	{
		if (asNoTracking)
		{
			return await TableNoTracking.ToListAsync(cancellationToken: cancellationToken);
		}

		return await Table.ToListAsync(cancellationToken: cancellationToken);
	}
	#endregion /Get

	#region Update
	public virtual void Update(TEntity entity, bool saveNow = true)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		Entities.Update(entity);

		if (saveNow)
			DatabaseContext.SaveChanges();
	}


	public virtual async Task UpdateAsync
		(TEntity entity, CancellationToken cancellationToken = default, bool saveNow = true)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		Entities.Update(entity);

		if (saveNow)
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}


	public virtual void UpdateRange
		(IEnumerable<TEntity> entities, bool saveNow = true)
	{
		Assert.NotNull(obj: entities, name: nameof(entities));

		Entities.UpdateRange(entities);

		if (saveNow)
			DatabaseContext.SaveChanges();
	}


	public virtual async Task UpdateRangeAsync
		(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool saveNow = true)
	{
		Assert.NotNull(obj: entities, name: nameof(entities));

		Entities.UpdateRange(entities);

		if (saveNow)
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion /Update

	#region Delete
	public virtual void Delete(TEntity entity, bool saveNow = true)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		Entities.Remove(entity);

		if (saveNow)
			DatabaseContext.SaveChanges();
	}


	public virtual async Task DeleteAsync
		(TEntity entity, CancellationToken cancellationToken = default, bool saveNow = true)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		Entities.Remove(entity);

		if (saveNow)
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}


	public virtual void DeleteRange
		(IEnumerable<TEntity> entities, bool saveNow = true)
	{
		Assert.NotNull(obj: entities, name: nameof(entities));

		Entities.RemoveRange(entities);

		if (saveNow)
			DatabaseContext.SaveChanges();
	}


	public virtual async Task DeleteRangeAsync
		(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
	{
		Assert.NotNull(obj: entities, name: nameof(entities));

		Entities.RemoveRange(entities);

		if (saveNow)
			await DatabaseContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion /Delete

	#region Attach & Detach
	public virtual void Detach(TEntity entity)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		var entry = DatabaseContext.Entry(entity);

		if (entry != null)
			entry.State = EntityState.Detached;
	}


	public virtual void Attach(TEntity entity)
	{
		Assert.NotNull(obj: entity, name: nameof(entity));

		if (DatabaseContext.Entry(entity).State == EntityState.Detached)
			Entities.Attach(entity);
	}
	#endregion /Attach&Detach

	#region SaveChanges
	public int SaveChange()
	{
		return DatabaseContext.SaveChanges();
	}


	public async Task<int> SaveChangesAsync()
	{
		return await DatabaseContext.SaveChangesAsync();
	}
	#endregion /SaveChanges
}
