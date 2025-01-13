using Domain.SeedWork;

namespace Infrastructure.Utilities;

public class RecursiveEntityUtilities<TEntityKey> : IRecursiveEntityUtilities<TEntityKey>
{
	private readonly DatabaseContext _databaseContext;

	public RecursiveEntityUtilities(DatabaseContext databaseContext)
	{
		_databaseContext = databaseContext;

		EntityPath = [];
	}

	public List<TEntityKey> EntityPath { get; set; }


	public async Task<TEntityKey?> GetEntityRootId<TEntity>
		(TEntityKey entityId) where TEntity : RecursiveEntity<TEntity, TEntityKey?>
	{
		if (entityId == null)
			return default;

		var current =
			await _databaseContext.Set<TEntity>()
			.Where(p => p.Id!.Equals(entityId))
			.Select(current => new
			{
				current.Id,
				current.ParentId
			})
			.FirstOrDefaultAsync();

		if (current == null)
			return default;

		if (current.ParentId is not null)
		{
			return await GetEntityRootId<TEntity>(current.ParentId);
		}

		return current.Id;
	}


	public async Task<string?> GetEntityPath<TEntity>
		(TEntityKey entityId) where TEntity : RecursiveEntity<TEntity, TEntityKey?>
	{
		var current =
			await _databaseContext.Set<TEntity>()
			.Where(p => p.Id!.Equals(entityId))
			.Select(current => new
			{
				current.Id,
				current.ParentId
			})
			.FirstOrDefaultAsync();

		if (current == null || current.Id == null)
			return default;

		EntityPath.Add(current.Id);

		if (current.ParentId is not null)
		{
			EntityPath.Add(current.ParentId);

			return await GetEntityPath<TEntity>(current.ParentId);
		}

		var entityPath = new StringBuilder();

		var sortedEntityPath = EntityPath.Distinct().Order().ToList();

		for (int i = 0; i < sortedEntityPath.Count; i++)
		{
			if (i != sortedEntityPath.Count - 1)
			{
				entityPath.Append($"{sortedEntityPath[i]}/");
			}
			else
			{
				entityPath.Append($"{sortedEntityPath[i]}");
			}
		}

		return entityPath.ToString();
	}


	public int GetEntityDepth<TEntity>(TEntity entity) where TEntity : RecursiveEntity<TEntity, TEntityKey?>
	{
		ArgumentException.ThrowIfNullOrEmpty(nameof(entity));

		if (entity.ParentId is not null && entity.Parent is not null)
		{
			return entity.Parent.Depth + 1;
		}

		return 1;
	}


	public async Task DeleteRecursiveEntity<TEntity>(TEntity entity) where TEntity : RecursiveEntity<TEntity, TEntityKey?>
	{
		if (entity.Children != null)
		{
			var children = await _databaseContext.Set<TEntity>()
				.Include(x => x.Children)
				.Where(x => x.ParentId!.Equals(entity.Id))
				.ToListAsync();

			foreach (var child in children)
			{
				await DeleteRecursiveEntity(child);
			}
		}

		_databaseContext.Remove(entity);
	}
}
