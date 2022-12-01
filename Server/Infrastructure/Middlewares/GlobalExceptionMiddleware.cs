namespace Infrastructure.Middlewares;

public class GlobalExceptionMiddleware
{
	public GlobalExceptionMiddleware(RequestDelegate next, Dtat.Logging.ILogger<GlobalExceptionMiddleware> logger) : base()
	{
		Next = next;
		Logger = logger;
	}


	protected RequestDelegate Next { get; }
	public Dtat.Logging.ILogger<GlobalExceptionMiddleware> Logger { get; }


	public async Task InvokeAsync(HttpContext httpContext)
	{
		try
		{
			await Next(httpContext);
		}
		catch (Exception ex)
		{
			//******************************
			Logger.LogCritical(ex, ex.Message);
			//******************************

			//******************************
			httpContext.Response.StatusCode = 500;

			httpContext.Response.ContentType = "application/json";

			var result = new Result();

			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UnkonwnError);

			result.AddErrorMessage(errorMessage);

			await httpContext.Response.WriteAsJsonAsync(result);
			//******************************
		}
	}
}
