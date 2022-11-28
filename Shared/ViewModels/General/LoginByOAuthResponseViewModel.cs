namespace ViewModels.General;

public class LoginByOAuthResponseViewModel
{
	public string? access_token { get; set; }
	public string? refresh_token { get; set; }
	public string? token_type { get; set; }
	public int? expires_in { get; set; }
	public string? username { get; set; }
}
