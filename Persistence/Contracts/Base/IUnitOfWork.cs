namespace Persistence;

public interface IUnitOfWork
{
	int SaveChange();

	Task<int> SaveChangesAsync();
}
