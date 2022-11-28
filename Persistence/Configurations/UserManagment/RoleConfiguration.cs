namespace Persistence.Configurations.UserManagment;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		//********************
		builder.Property
			(current => current.Id)
				.ValueGeneratedOnAdd();
		//********************


		//********************
		builder.Property
			(current => current.Title)
				.HasMaxLength(maxLength: 25)
				.IsRequired();
		//********************


		//********************
		builder.Property
			(current => current.Price)
				.IsRequired(false);
		//********************


		//********************
		builder.HasData(new List<Role>()
		{
			new Role(title: "SystemAdministrator")
			{
				Id = 1,
			},
			new Role(title: "Admin")
			{
				Id = 2,
			},
			new Role(title: "User")
			{
				Id = 3,
			},
		});
		//********************
	}
}
