namespace ViewModels.General;

public class LoginViewModel
{
	public LoginViewModel(string username, int roleId)
	{
		Username = username;
		RoleId = roleId;
	}

	public int Id { get; set; }
	public int RoleId { get; set; }
	public string? Email { get; set; }
	public bool IsBanned { get; set; }
	public string Username { get; set; }
	public Guid SecurityStamp { get; set; }
}
