namespace ViewModels.General;

public class LoginByOAuthRequestViewModel
{
	[Required]
	public string? Grant_Type { get; set; }

	[Required]
	public string? Username { get; set; }

	[Required]
	public string? Password { get; set; }

	public string? Refresh_Token { get; set; }
	public string? Scope { get; set; }

	public string? Client_Id { get; set; }
	public string? Client_Secret { get; set; }
}
