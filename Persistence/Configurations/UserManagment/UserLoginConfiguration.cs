namespace Persistence.Configurations.UserManagment;

internal class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
	public void Configure(EntityTypeBuilder<UserLogin> builder)
	{
		//********************
		builder.HasIndex
			(current => current.RefreshToken)
				.IsUnique();
		//********************
	}
}
