namespace ViewModels.General;

public class LoginViewModel
{
	public LoginViewModel(string username, int roleId, string roleName)
	{
		UserName = username;
		RoleId = roleId;
		RoleName = roleName;
	}

	public int Id { get; set; }
	public int RoleId { get; set; }
	public string? Email { get; set; }
	public bool IsBanned { get; set; }
	public string RoleName { get; set; }
	public string UserName { get; set; }
	public required string SecurityStamp { get; set; }
}
