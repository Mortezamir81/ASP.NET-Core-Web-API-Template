namespace ViewModels.General;

public class LoginByOAuthRequestViewModel
{
	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public string? Grant_Type { get; set; }

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

	public string? Refresh_Token { get; set; }
	public string? Scope { get; set; }

	public string? Client_Id { get; set; }
	public string? Client_Secret { get; set; }
}
