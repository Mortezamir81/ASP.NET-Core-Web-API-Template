namespace ViewModels.General;

public class ResetPasswordRequestViewModel
{
	public ResetPasswordRequestViewModel() : base()
	{
	}

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public string? Password { get; set; }
}
