namespace Infrastructure;

[ApiController]
[Route("api/[controller]")]
public class BaseControllerWithDatabase : ControllerBase
{
	public BaseControllerWithDatabase(DatabaseContext databaseContext)
	{
		DatabaseContext = databaseContext;
	}

	public DatabaseContext DatabaseContext { get; }
}
