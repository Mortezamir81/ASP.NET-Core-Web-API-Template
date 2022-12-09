namespace Persistence;

public class DatabaseContext : DbContext
{
	public DatabaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
	{
#if DEBUG
		Database.EnsureCreated();
#endif
	}


	public DbSet<User>? Users { get; set; }
	public DbSet<UserLogin>? UserLogins { get; set; }
	public DbSet<Role>? Roles { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
	}
}
