using Asp.Versioning;

namespace Infrastructure;

[ApiController]
[ApiVersion("1")]
[Produces("application/json")]
[Route("api/[controller]")]
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


	[NonAction]
	protected int GetRequiredUserId()
	{
		if (User?.Identity?.IsAuthenticated == false)
			throw new Exception("The user is not authenticated!");

		var stringUserId =
			User?.Claims.FirstOrDefault(current => current.Type == ClaimTypes.NameIdentifier)?.Value;

		if (stringUserId == null)
			throw new Exception("The user is not authenticated!");

		var userId = int.Parse(stringUserId);

		return userId;
	}
}
