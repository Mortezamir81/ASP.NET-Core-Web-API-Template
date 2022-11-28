namespace Infrastructure;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseControllerWithDatabase : ControllerBase
{
	public BaseControllerWithDatabase(DatabaseContext databaseContext)
	{
		DatabaseContext = databaseContext;
	}

	public DatabaseContext DatabaseContext { get; }
}
