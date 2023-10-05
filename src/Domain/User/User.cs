namespace Domain;

public class User : IdentityUser<int>, IEntityHasIsSystemic
{
	public User(string userName)
	{
		UserName = userName;
	}

	public bool IsBanned { get; set; }

	public bool IsDeleted { get; set; }

	public string? FullName { get; set; }

	public bool IsSystemic { get; set; }

	public List<UserToken>? UserToken { get; set; }
}
