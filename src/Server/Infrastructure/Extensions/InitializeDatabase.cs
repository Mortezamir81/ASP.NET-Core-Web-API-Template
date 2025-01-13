namespace Infrastructure.Extensions;

public static class InitializeDatabase
{
	public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
	{
		using var scope = app.ApplicationServices.CreateScope();

		var databaseContext =
			scope.ServiceProvider.GetRequiredService<DatabaseContext>();

		var userManager =
			scope.ServiceProvider.GetRequiredService<UserManager<User>>();

		var roleManager =
			scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

		var anyUserExist =
			await databaseContext.Users
			.AnyAsync();

		if (anyUserExist)
			return;

		var systemAdminRoleResult =
			await roleManager.CreateAsync(new Role(Constants.Role.SystemAdmin));

		if (!systemAdminRoleResult.Succeeded)
			throw new Exception(message: $"Can not create role {Constants.Role.SystemAdmin} - {GetErrors(systemAdminRoleResult.Errors)}");

		var adminRoleResult =
			await roleManager.CreateAsync(new Role(Constants.Role.Admin));

		if (!adminRoleResult.Succeeded)
			throw new Exception(message: $"Can not create role {Constants.Role.Admin}- {GetErrors(adminRoleResult.Errors)}");

		var userRoleResult =
			await roleManager.CreateAsync(new Role(Constants.Role.User));

		if (!userRoleResult.Succeeded)
			throw new Exception(message: $"Can not create role {Constants.Role.User} - {GetErrors(userRoleResult.Errors)}");

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

		var systemAdminUser =
			await userManager.CreateAsync(adminUser, password: "morteza@12345");

		if (!systemAdminUser.Succeeded)
			throw new Exception(message: $"Can not create user {adminUser.UserName} - {GetErrors(systemAdminUser.Errors)}");

		var adminUserToRole =
			await userManager.AddToRoleAsync(user: adminUser, role: Constants.Role.SystemAdmin);

		if (!adminUserToRole.Succeeded)
			throw new Exception(message: $"Can not add role {Constants.Role.SystemAdmin} to user {adminUser.UserName} - {GetErrors(adminUserToRole.Errors)}");
	}

	public static string GetErrors(IEnumerable<IdentityError> errors)
	{
		var finalResult = new StringBuilder();

		foreach (var error in errors)
		{
			finalResult.AppendLine(error.Description);
		}

		return finalResult.ToString();
	}
}
