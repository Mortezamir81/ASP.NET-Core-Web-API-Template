namespace Constants;

public static class Role : object
{
	static Role()
	{
	}

	public const string User = "کاربر";
	public const string Admin = "ادمین";
	public const string SystemAdmin = "ادمین سیستم";

	public const int SystemAdminRoleId = 1;
	public const int AdminRoleId = 2;
	public const int UserRoleId = 3;
}
