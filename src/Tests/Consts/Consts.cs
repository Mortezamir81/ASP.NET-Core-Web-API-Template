namespace Tests;

internal class Consts
{
	internal class General
	{
		public const string ConnectionString =
			@"Data Source=.;Initial Catalog=Templete_Test;Integrated Security=true; TrustServerCertificate=True";
	}

	internal class UserServices
	{
		public const int SystemAdminId = 1;
		public const int SecoundSystemAdminId = 2;
		public const int AdminId = 3;
		public const int SecoundAdminId = 4;
		public const int UserId = 5;
		public const int BanUserId = 6;
		public const int UserForDeleteId = 7;
		public const int UserForEditRoleId = 8;
		public const int UserForUpdateId = 9;

		public const string SystemAdminUserName = "SystemAdminUser";
		public const string SecoundSystemAdminUserName = "SecoundSystemAdminUser";
		public const string AdminUserName = "AdminUser";
		public const string SecoundAdminUserName = "SecoundAdminUser";
		public const string UserUserName = "NormalUser";
		public const string BanUserUserName = "BanUser";
		public const string UserForDeleteUserName = "UserForDelete";
		public const string UserForEditRoleUserName = "UserForEditRole";
		public const string UserForUpdateUserName = "UserForUpdate";

		public const string SystemAdminEmail = "SystemAdminUser@gmail.com";
		public const string SecoundSystemAdminEmail = "SecoundSystemAdminUser@gmail.com";
		public const string AdminEmail = "AdminUser@gmail.com";
		public const string SecoundAdminEmail = "SecoundAdminUser@gmail.com";
		public const string UserEmail = "NormalUser@gmail.com";
		public const string BanUserEmail = "BanUser@gmail.com";
		public const string UserForDeleteEmail = "UserForDelete@gmail.com";
		public const string UserForEditRoleEmail = "UserForEditRole@gmail.com";
		public const string UserForUpdateEmail = "UserForUpdate@gmail.com";

		public const string UsersPassword = "morteza@12345";
	}
}
