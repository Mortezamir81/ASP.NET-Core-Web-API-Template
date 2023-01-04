namespace Tests.Fixtures;

public class RegistrationServices : IDisposable
{
	#region Properties
	public IServiceCollection Services { get; set; }

	public IServiceProvider ServiceProvider { get; set; }
	#endregion /Properties

	#region Constractor
	public RegistrationServices()
	{
		Services = new ServiceCollection();

		var configuration =
			new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();

		Services.Configure<ApplicationSettings>
			(configuration.GetSection(ApplicationSettings.KeyName));

		var identitySettings =
			configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(IdentitySettings)}").Get<IdentitySettings>();

		Services.AddCustomIdentity(identitySettings);

		Services.AddCustomDbContext(connectionString: Consts.ConnectionString);

		Services.AddLogging(config => config.AddConsole());

		ServiceProvider = Services.BuildServiceProvider();

		SeedData().GetAwaiter().GetResult();
	}
	#endregion /Constrctor

	#region Methods
	public void Dispose()
	{
		var dbContext =
			ServiceProvider.GetService<DatabaseContext>();

		dbContext?.Database.EnsureDeleted();
	}


	public async Task SeedData()
	{
		var dbContext =
			ServiceProvider.GetRequiredService<DatabaseContext>();

		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();

		var userManager =
			ServiceProvider.GetService<UserManager<User>>();

		var roleManager =
			ServiceProvider.GetService<RoleManager<Role>>();

		Common.Utilities.Assert.NotNull(userManager, name: nameof(userManager));
		Common.Utilities.Assert.NotNull(roleManager, name: nameof(roleManager));

		await roleManager!.CreateAsync(new Role(Constants.Role.SystemAdmin));
		await roleManager!.CreateAsync(new Role(Constants.Role.Admin));
		await roleManager!.CreateAsync(new Role(Constants.Role.User));

		var systemAdminUser =
			new User(Consts.SystemAdminUsername)
			{
				Email = Consts.SystemAdminEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var adminUser =
			new User(Consts.AdminUsername)
			{
				Email = Consts.AdminEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var user =
			new User(Consts.UserUsername)
			{
				Email = Consts.UserEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};


		var banUser =
			new User(Consts.BanUserUsername)
			{
				Email = Consts.BanUserEmail,
				EmailConfirmed = true,
				FullName = "some name",
				IsBanned = true,
			};

		var userForDelete =
			new User(Consts.UserForDeleteUsername)
			{
				Email = Consts.UserForDeleteEmail,
				EmailConfirmed = true,
				FullName = "some name",
				IsBanned = false,
			};

		await userManager!.CreateAsync(systemAdminUser, password: Consts.UsersPassword);
		await userManager!.CreateAsync(adminUser, password: Consts.UsersPassword);
		await userManager!.CreateAsync(user, password: Consts.UsersPassword);
		await userManager!.CreateAsync(banUser, password: Consts.UsersPassword);
		await userManager!.CreateAsync(userForDelete, password: Consts.UsersPassword);

		await userManager.AddToRoleAsync(user: systemAdminUser, role: Constants.Role.SystemAdmin);
		await userManager.AddToRoleAsync(user: adminUser, role: Constants.Role.Admin);
		await userManager.AddToRoleAsync(user: user, role: Constants.Role.User);
		await userManager.AddToRoleAsync(user: banUser, role: Constants.Role.User);
		await userManager.AddToRoleAsync(user: userForDelete, role: Constants.Role.User);
	}
	#endregion /Methods
}
