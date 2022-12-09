namespace Tests;

public class DatabaseFixture : IDisposable
{
	#region Fields
	private const string ConnectionString =
		@"Data Source=.;Initial Catalog=Templete_Test;Integrated Security=true; TrustServerCertificate=True";

	private static readonly object _lock = new();
	private static bool _databaseInitialized;
	#endregion /Fields

	#region Constractor
	public DatabaseFixture() : base()
	{
		lock (_lock)
		{
			if (!_databaseInitialized)
			{
				using (var context = CreateContext())
				{
					context.Database.EnsureDeleted();
					context.Database.EnsureCreated();

					SeedData(context);
				}

				_databaseInitialized = true;
			}
		}
	}
	#endregion /Constractor

	#region Methods
	public void Dispose()
	{
		using (var context = CreateContext())
		{
			context.Database.EnsureDeleted();
		}
	}


	public DatabaseContext CreateContext()
	{
		var dbOptions =
			new DbContextOptionsBuilder<DatabaseContext>()
			.UseSqlServer(ConnectionString)
			.Options;

		return new DatabaseContext(dbOptions);
	}


	public void SeedData(DatabaseContext dbContext)
	{
		var roles = new List<Role>()
		{
			new Role(title: Constants.Role.SystemAdmin),
			new Role(title: Constants.Role.Admin),
			new Role(title: Constants.Role.User),
		};

		dbContext.Roles?.AddRange(roles);
		dbContext.SaveChanges();

		var users =
			new List<User>()
			{
				new User(Consts.SystemAdminUsername)
				{
					RoleId = (int)UserRoleEnum.SystemAdministrator,
					HashedPassword = Dtat.Utilities.Security.GetSha256(text: Consts.UsersPassword),
					SecurityStamp = Guid.NewGuid(),
				},
				new User(Consts.AdminUsername)
				{
					RoleId = (int)UserRoleEnum.Admin,
					HashedPassword = Dtat.Utilities.Security.GetSha256(text: Consts.UsersPassword),
					SecurityStamp = Guid.NewGuid(),
				},
				new User(Consts.UserUsername)
				{
					RoleId = (int)UserRoleEnum.User,
					HashedPassword = Dtat.Utilities.Security.GetSha256(text: Consts.UsersPassword),
					SecurityStamp = Guid.NewGuid(),
				},
				new User(Consts.BanUserUsername)
				{
					RoleId = (int)UserRoleEnum.User,
					HashedPassword = Dtat.Utilities.Security.GetSha256(text: Consts.UsersPassword),
					IsBanned = true,
					SecurityStamp = Guid.NewGuid(),
				},
			};

		dbContext.Users?.AddRange(users);
		dbContext.SaveChanges();
	}
	#endregion /Methods
}