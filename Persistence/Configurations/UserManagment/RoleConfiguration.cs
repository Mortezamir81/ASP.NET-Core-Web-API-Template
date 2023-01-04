namespace Persistence.Configurations.UserManagment;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		//********************
		//builder.HasData(new List<Role>()
		//{
		//	new Role(name: Constants.Role.SystemAdmin)
		//	{
		//		Id = 1,
		//	},
		//	new Role(name: Constants.Role.Admin)
		//	{
		//		Id = 2,
		//	},
		//	new Role(name: Constants.Role.User)
		//	{
		//		Id = 3,
		//	},
		//});
		//********************
	}
}
