namespace ViewModels.General;

public class LoginResponseViewModel
{
	public string? Token { get; set; }
	public string? UserName { get; set; }
	public Guid RefreshToken { get; set; }
	public long ExpiresIn { get; set; }
}
