namespace ViewModels.General;

public class UpdateUserByAdminRequestViewModel
{
	[Required
	(AllowEmptyStrings = false,
	ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
	ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]

	[MinValue(1,
		ErrorMessageResourceType = typeof(Softmax.Utilities.Validation.Messages.ValidationErros),
		ErrorMessageResourceName = nameof(Softmax.Utilities.Validation.Messages.ValidationErros.MinValue))]
	public int? UserId { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]
	public string? Email { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]
	public string? FullName { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(Resources.Messages.ErrorMessages),
		ErrorMessageResourceName = nameof(Resources.Messages.ErrorMessages.MostNotBeNull))]
	public string? Username { get; set; }
}
