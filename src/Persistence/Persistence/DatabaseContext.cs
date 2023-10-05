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
	public DbSet<UserToken>? UserAccessTokens { get; set; }
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

		AddDateTimeOffsetSupportToSqllite(modelBuilder);
	}

	private void AddDateTimeOffsetSupportToSqllite(ModelBuilder modelBuilder)
	{
		if (Database.IsSqlite())
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				var properties =
					entityType.ClrType.GetProperties()
					.Where(current =>
						current.PropertyType == typeof(DateTimeOffset) ||
						current.PropertyType == typeof(DateTimeOffset?));

				foreach (var property in properties)
				{
					modelBuilder
						.Entity(name: entityType.Name)
						.Property(propertyName: property.Name)
						.HasConversion(converter:
							new Microsoft.EntityFrameworkCore
							.Storage.ValueConversion.DateTimeOffsetToBinaryConverter());
				}
			}
		}
	}
	#endregion /Methods
}
