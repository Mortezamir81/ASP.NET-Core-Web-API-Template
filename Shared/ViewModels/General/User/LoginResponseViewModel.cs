namespace ViewModels.General;

public class LoginResponseViewModel
{
	public string? Token { get; set; }
	public string? Username { get; set; }
	public Guid RefreshToken { get; set; }
}
