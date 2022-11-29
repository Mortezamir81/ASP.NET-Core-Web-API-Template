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
			new Role(title: Constants.Role.SystemAdmin)
			{
				Id = 1,
			},
			new Role(title: Constants.Role.Admin)
			{
				Id = 2,
			},
			new Role(title: Constants.Role.User)
			{
				Id = 3,
			},
		});
		//********************
	}
}
