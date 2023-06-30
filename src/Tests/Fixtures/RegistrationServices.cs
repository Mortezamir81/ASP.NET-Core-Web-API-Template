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

		Services.AddCustomDbContext(connectionString: Consts.General.ConnectionString);

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

		Softmax.Utilities.Validation.Assert.NotNull(userManager, name: nameof(userManager));
		Softmax.Utilities.Validation.Assert.NotNull(roleManager, name: nameof(roleManager));

		await roleManager!.CreateAsync(new Role(Constants.Role.SystemAdmin));
		await roleManager!.CreateAsync(new Role(Constants.Role.Admin));
		await roleManager!.CreateAsync(new Role(Constants.Role.User));

		var systemAdminUser =
			new User(Consts.UserServices.SystemAdminUserName)
			{
				Email = Consts.UserServices.SystemAdminEmail,
				EmailConfirmed = true,
				FullName = "some name",
				IsSystemic = true,
			};

		var secoundSystemAdminUser =
			new User(Consts.UserServices.SecoundSystemAdminUserName)
			{
				Email = Consts.UserServices.SecoundSystemAdminEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var adminUser =
			new User(Consts.UserServices.AdminUserName)
			{
				Email = Consts.UserServices.AdminEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var secoundAdminUser =
			new User(Consts.UserServices.SecoundAdminUserName)
			{
				Email = Consts.UserServices.SecoundAdminEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var user =
			new User(Consts.UserServices.UserUserName)
			{
				Email = Consts.UserServices.UserEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var banUser =
			new User(Consts.UserServices.BanUserUserName)
			{
				Email = Consts.UserServices.BanUserEmail,
				EmailConfirmed = true,
				FullName = "some name",
				IsBanned = true,
			};

		var userForDelete =
			new User(Consts.UserServices.UserForDeleteUserName)
			{
				Email = Consts.UserServices.UserForDeleteEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var userForEditRole =
			new User(Consts.UserServices.UserForEditRoleUserName)
			{
				Email = Consts.UserServices.UserForEditRoleEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		var userForUpdate =
			new User(Consts.UserServices.UserForUpdateUserName)
			{
				Email = Consts.UserServices.UserForUpdateEmail,
				EmailConfirmed = true,
				FullName = "some name",
			};

		await userManager!.CreateAsync(systemAdminUser, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(secoundSystemAdminUser, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(adminUser, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(secoundAdminUser, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(user, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(banUser, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(userForDelete, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(userForEditRole, password: Consts.UserServices.UsersPassword);
		await userManager!.CreateAsync(userForUpdate, password: Consts.UserServices.UsersPassword);

		await userManager.AddToRoleAsync(user: systemAdminUser, role: Constants.Role.SystemAdmin);
		await userManager.AddToRoleAsync(user: secoundSystemAdminUser, role: Constants.Role.SystemAdmin);
		await userManager.AddToRoleAsync(user: adminUser, role: Constants.Role.Admin);
		await userManager.AddToRoleAsync(user: secoundAdminUser, role: Constants.Role.Admin);
		await userManager.AddToRoleAsync(user: user, role: Constants.Role.User);
		await userManager.AddToRoleAsync(user: banUser, role: Constants.Role.User);
		await userManager.AddToRoleAsync(user: userForDelete, role: Constants.Role.User);
		await userManager.AddToRoleAsync(user: userForEditRole, role: Constants.Role.User);
		await userManager.AddToRoleAsync(user: userForUpdate, role: Constants.Role.User);
	}
	#endregion /Methods
}
