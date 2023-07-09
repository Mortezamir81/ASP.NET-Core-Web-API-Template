namespace Domain.SeedWork;

public abstract class RecursiveEntity<TEntity, TKeyType>
	: BaseEntity, IRecursiveEntity<TEntity, TKeyType>
{
	public required TKeyType ParentId { get; set; }

	public required TEntity Parent { get; set; }

	public required TKeyType RootId { get; set; }

	public int Depth { get; set; }

	public string? RecursivePath { get; set; }

	public required IList<TEntity> Children { get; set; }
}


public interface IRecursiveEntity<TEntity, TKeyType>
{
	TKeyType ParentId { get; set; }

	TEntity Parent { get; set; }

	TKeyType RootId { get; set; }

	int Depth { get; set; }

	string? RecursivePath { get; }

	IList<TEntity> Children { get; set; }
}
