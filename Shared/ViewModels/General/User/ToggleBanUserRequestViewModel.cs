namespace ViewModels.General;

public class ToggleBanUserRequestViewModel
{
	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]

	[MinValue(1,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MinValue))]
	public int? UserId { get; set; }
}
