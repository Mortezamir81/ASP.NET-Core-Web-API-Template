namespace Constants;

public static class Role : object
{
	static Role()
	{
	}

	public const string User = "User";
	public const string Admin = "Admin";
	public const string SystemAdmin = "SystemAdmin";

	public const int SystemAdminRoleId = 1;
	public const int AdminRoleId = 2;
	public const int UserRoleId = 3;
}
