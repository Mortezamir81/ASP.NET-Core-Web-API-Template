using System.Threading.Tasks;

namespace Dtat.Data.EntityFrameworkCore
{
	public abstract class UnitOfWork<TDbContext> :
		IUnitOfWork where TDbContext : Microsoft.EntityFrameworkCore.DbContext
	{
		public UnitOfWork(TDbContext databaseContext) : base()
		{
			DatabaseContext = databaseContext;
		}

		protected TDbContext DatabaseContext { get; }

		public bool IsDisposed { get; protected set; }

		public void Dispose()
		{
			Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed)
			{
				return;
			}

			if (disposing)
			{
				if (DatabaseContext != null)
				{
					DatabaseContext.Dispose();
				}
			}

			IsDisposed = true;
		}

		public virtual async Task<int> SaveAsync()
		{
			return await DatabaseContext.SaveChangesAsync();
		}

		~UnitOfWork()
		{
			Dispose(false);
		}
	}
}
