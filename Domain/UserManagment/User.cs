namespace Domain.UserManagment;

public class User : BaseEntity
{
	public User(string username)
	{
		Username = username;
	}

	public bool IsBanned { get; set; }
	public string? Email { get; set; }
	public int? RoleId { get; set; }
	public bool IsDeleted { get; set; }
	public string? FullName { get; set; }
	public string Username { get; set; }
	public Role? UserRole { get; set; }
	public Guid? SecurityStamp { get; set; }
	public string? HashedPassword { get; set; }
	public List<UserLogin>? UserLogins { get; set; }
}
