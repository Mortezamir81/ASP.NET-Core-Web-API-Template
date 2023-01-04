namespace Infrastructure.Extentions;

public static class InitializeDatabase
{
	public static async Task IntializeDatabase(this IApplicationBuilder app)
	{
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var userManager = 
				scope.ServiceProvider.GetService<UserManager<User>>();

			var roleManager = 
				scope.ServiceProvider.GetService<RoleManager<Role>>();

			Assert.NotNull(userManager, name: nameof(userManager));
			Assert.NotNull(roleManager, name: nameof(roleManager));

			await roleManager!.CreateAsync(new Role(Constants.Role.SystemAdmin));
			await roleManager!.CreateAsync(new Role(Constants.Role.Admin));
			await roleManager!.CreateAsync(new Role(Constants.Role.User));

			var adminUser =
				new User("Morteza.m")
				{
					Email = "admin@gmail.com",
					EmailConfirmed = true,
					PhoneNumberConfirmed = true,
					FullName = "admin",
					PhoneNumber = "09165059874",
					IsSystemic = true,
				};

			await userManager!.CreateAsync(adminUser, password: "morteza@12345");

			await userManager.AddToRoleAsync(user: adminUser, role: Constants.Role.SystemAdmin);
		}
	}
}
