namespace Persistence.Configurations.UserManagment;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		//********************
		builder.HasQueryFilter
			(current => !current.IsDeleted);
		//********************


		//********************
		builder.HasIndex
			(current => current.Email)
				.IsUnique();

		builder.HasIndex
			(current => current.Username)
				.IsUnique();

		builder.Property
			(current => current.Username)
				.HasMaxLength(maxLength: Constants.MaxLength.CellPhoneNumber)
				.IsRequired();

		builder.Property
			(current => current.Email)
				.HasMaxLength(maxLength: Constants.MaxLength.EmailAddress)
				.IsRequired(false);

		builder.Property
			(current => current.FullName)
				.HasMaxLength(maxLength: Constants.MaxLength.FullName)
				.IsRequired(false);

		builder.Property
			(current => current.HashedPassword)
				.HasMaxLength(maxLength: 64)
				.IsRequired();

		builder.Property
			(current => current.RoleId)
				.IsRequired();
		//********************


		//********************
		builder.Property
			(current => current.IsBanned)
			.HasDefaultValue(false)
			.IsRequired();

		builder.Property
		(current => current.IsDeleted)
			.HasDefaultValue(false)
			.IsRequired();

		builder.Property
			(current => current.CreatedDate)
				.HasDefaultValueSql("getutcdate()")
				.HasColumnType("datetime")
				.IsRequired();
		//********************


		//********************
		builder.HasData(new User(username: "Mirshekar")
		{
			Id = 1,
			Email = "asreweb81@gmail.com",
			HashedPassword = "65899312236121168271922016612431885116324511121135",
			RoleId = 1,
			FullName = "Morteza Mirshekar",
			SecurityStamp = Guid.NewGuid()
		});
		//********************
	}
}
