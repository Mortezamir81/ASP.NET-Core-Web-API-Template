namespace Persistence.Configurations;

internal class UserLoginConfiguration : IEntityTypeConfiguration<UserToken>
{
	public void Configure(EntityTypeBuilder<UserToken> builder)
	{
		//********************
		builder.HasIndex
			(current => current.AccessTokenHash)
				.IsUnique();
		//********************
	}
}
