namespace ViewModels.General;

public class ToggleBanUserRequestViewModel
{
	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]

	[MinValue(1,
		ErrorMessageResourceType = typeof(Softmax.Utilities.Validation.Messages.ValidationErros),
		ErrorMessageResourceName = nameof(Softmax.Utilities.Validation.Messages.ValidationErros.MinValue))]
	public int? UserId { get; set; }
}
