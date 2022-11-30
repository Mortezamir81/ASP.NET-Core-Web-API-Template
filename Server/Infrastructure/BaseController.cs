namespace Infrastructure;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{

}
