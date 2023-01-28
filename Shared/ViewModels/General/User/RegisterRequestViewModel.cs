namespace ViewModels.General;

public class RegisterRequestViewModel
{
	public RegisterRequestViewModel() : base()
	{
	}

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]
	public string? Username { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]
	public string? Email { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]
	public string? Password { get; set; }
}
