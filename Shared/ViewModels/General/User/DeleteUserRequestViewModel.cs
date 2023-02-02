namespace ViewModels.General;

public class DeleteUserRequestViewModel
{
	[Required
		(AllowEmptyStrings = false,
		ErrorMessageResourceType = typeof(ValidationErros),
		ErrorMessageResourceName = nameof(ValidationErros.MostNotBeNull))]
	public Guid? Id { get; set; }
}
