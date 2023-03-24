namespace ViewModels.General;

public class UpdateUserByAdminRequestViewModel
{
	[Required
	(AllowEmptyStrings = false,
	ErrorMessageResourceType = typeof(ValidationErros),
	ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]

	[MinValue(1,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MinValue))]
	public int? UserId { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public string? Email { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public string? FullName { get; set; }

	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public string? UserName { get; set; }
}
