namespace Persistence.Repositories.Base;

public interface IRepositoryBase<TEntity> where TEntity : class
{
	#region Properties
	DbSet<TEntity> Entities { get; }
	IQueryable<TEntity> Table { get; }
	IQueryable<TEntity> TableNoTracking { get; }
	#endregion /Properties

	#region Add
	void Add(TEntity entity, bool saveNow = true);
	Task AddAsync(TEntity entity, CancellationToken cancellationToken = default, bool saveNow = true);


	void AddRange(IEnumerable<TEntity> entities, bool saveNow = true);
	Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool saveNow = true);
	#endregion /Add

	#region Get
	TEntity? GetById(params object[] ids);

	Task<TEntity?> GetByIdAsync
		(CancellationToken cancellationToken = default, params object[] ids);

	Task<IEnumerable<TEntity>> GetAllAsync
		(bool asNoTracking = false, CancellationToken cancellationToken = default);
	#endregion /Get

	#region Update
	void Update(TEntity entity, bool saveNow = true);

	Task UpdateAsync
		(TEntity entity, CancellationToken cancellationToken = default, bool saveNow = true);

	void UpdateRange
		(IEnumerable<TEntity> entities, bool saveNow = true);

	Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool saveNow = true);
	#endregion /Update

	#region Delete
	void Delete(TEntity entity, bool saveNow = true);

	Task DeleteAsync
		(TEntity entity, CancellationToken cancellationToken = default, bool saveNow = true);

	void DeleteRange
		(IEnumerable<TEntity> entities, bool saveNow = true);

	Task DeleteRangeAsync
		(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool saveNow = true);
	#endregion /Delete

	#region Attach & Detach
	void Attach(TEntity entity);
	
	void Detach(TEntity entity);
	#endregion /Attach&Detach

	#region SaveChanges
	public int SaveChange();

	Task<int> SaveChangesAsync();
	#endregion /SaveChanges
}