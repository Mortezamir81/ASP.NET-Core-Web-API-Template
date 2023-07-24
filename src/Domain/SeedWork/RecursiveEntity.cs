namespace Domain.SeedWork;

public abstract class RecursiveEntity<TEntity, TEntityKey>
	: BaseEntity<TEntityKey>, IRecursiveEntity<TEntity, TEntityKey>
{
	public TEntityKey? ParentId { get; set; }

	public TEntity? Parent { get; set; }

	public TEntityKey? RootId { get; set; }

	public int Depth { get; set; }

	public string? RecursivePath { get; set; }

	public IList<TEntity>? Children { get; set; }
}


public interface IRecursiveEntity<TEntity, TEntityKey>
{
	TEntityKey? ParentId { get; set; }

	TEntity? Parent { get; set; }

	TEntityKey? RootId { get; set; }

	int Depth { get; set; }

	string? RecursivePath { get; }

	IList<TEntity>? Children { get; set; }
}
