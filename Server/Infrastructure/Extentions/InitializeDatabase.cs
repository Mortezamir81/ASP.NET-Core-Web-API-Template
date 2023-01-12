using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Extentions;

public static class InitializeDatabase
{
	public static async Task IntializeDatabase(this IApplicationBuilder app)
	{
		using (var scope = app.ApplicationServices.CreateScope())
		{
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

			await roleManager.CreateAsync(new Role(Constants.Role.SystemAdmin));
			await roleManager.CreateAsync(new Role(Constants.Role.Admin));
			await roleManager.CreateAsync(new Role(Constants.Role.User));

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

			await userManager.CreateAsync(adminUser, password: "morteza@12345");

			await userManager.AddToRoleAsync(user: adminUser, role: Constants.Role.SystemAdmin);
		}
	}
}
