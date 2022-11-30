namespace Persistence;

public class UnitOfWork : IUnitOfWork
{
	public UnitOfWork(DatabaseContext databaseContext)
	{
		DatabaseContext = databaseContext;
	}

	public DatabaseContext DatabaseContext { get; }


	public int SaveChange()
	{
		return DatabaseContext.SaveChanges();
	}

	public async Task<int> SaveChangesAsync()
	{
		return await DatabaseContext.SaveChangesAsync();
	}
}
