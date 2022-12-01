namespace Infrastructure;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{
	public BaseController() : base()
	{
	}


	[NonAction]
	protected int? GetUserId()
	{
		if (User?.Identity?.IsAuthenticated == false)
			return default;

		var stringUserId =
			User?.Claims.FirstOrDefault(current => current.Type == ClaimTypes.NameIdentifier)?.Value;

		if (stringUserId == null)
			return default;

		var userId = int.Parse(stringUserId);

		return userId;
	}
}
