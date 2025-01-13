using Domain.SeedWork;

namespace Infrastructure.Utilities;

public interface IRecursiveEntityUtilities<TEntityKey>
{
	public List<TEntityKey> EntityPath { get; set; }


	Task<TEntityKey?> GetEntityRootId<TEntity>
		(TEntityKey entityId) where TEntity : RecursiveEntity<TEntity, TEntityKey?>;


	Task<string?> GetEntityPath<TEntity>
		(TEntityKey entityId) where TEntity : RecursiveEntity<TEntity, TEntityKey?>;


	int GetEntityDepth<TEntity>(TEntity entity) where TEntity : RecursiveEntity<TEntity, TEntityKey?>;


	Task DeleteRecursiveEntity<TEntity>(TEntity entity) where TEntity : RecursiveEntity<TEntity, TEntityKey?>;
}
