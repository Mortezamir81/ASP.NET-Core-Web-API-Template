using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistence;

public class DatabaseContext : IdentityDbContext<User, Role, int>
{
	#region Constractor
	public DatabaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
	{
#if DEBUG
		Database.EnsureCreated();
#endif
	}
	#endregion /Constractor

	#region Properties
	public DbSet<UserLogin>? UserLogins { get; set; }
	#endregion /Properties

	#region Methods
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
	}
	#endregion /Methods
}
