using Resources.Messages;

namespace ViewModels.General;

public class ForgotPasswordRequestViewModel
{
	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]

	[RegularExpression
		(pattern: Constants.RegularExpression.EmailAddress,
		ErrorMessageResourceType = typeof(ErrorMessages),
		ErrorMessageResourceName = nameof(ErrorMessages.InvalidEmailStructure))]
	public string? Email { get; set; }
}
