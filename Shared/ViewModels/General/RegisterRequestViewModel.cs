namespace ViewModels.General;

public class RegisterRequestViewModel
{
	public RegisterRequestViewModel() : base()
	{
	}

	public string? Password { get; set; }
	public string? Username { get; set; }
	public string? Email { get; set; }
}
