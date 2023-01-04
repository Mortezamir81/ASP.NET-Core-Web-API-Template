namespace Domain.UserManagment;

public class User : IdentityUser<int>
{
	public User(string userName)
	{
		UserName = userName;
	}

	public bool IsBanned { get; set; }

	public bool IsDeleted { get; set; }

	public string? FullName { get; set; }

	public List<UserLogin>? UserLogins { get; set; }
}
