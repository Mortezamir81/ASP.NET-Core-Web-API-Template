namespace ViewModels.General;

public class LoginRequestViewModel
{
	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public string? UserName { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public string? Password { get; set; }
}
