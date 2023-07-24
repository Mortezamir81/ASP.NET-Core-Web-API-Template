using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistence;

public class DatabaseContext : IdentityDbContext<User, Role, int>
{
	#region Constractor
	public DatabaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
	{
	}
	#endregion /Constractor

	#region Properties
	public DbSet<UserLogin>? UserLogins { get; set; }
	#endregion /Properties

	#region Methods
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		var entitiesWithForeignKeys =
			modelBuilder.Model
			.GetEntityTypes()
			.SelectMany(currrent => currrent.GetForeignKeys());

		foreach (var entity in entitiesWithForeignKeys)
		{
			entity.DeleteBehavior = DeleteBehavior.Cascade;
		}

		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
	}
	#endregion /Methods
}
