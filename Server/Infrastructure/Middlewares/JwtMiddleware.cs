namespace Infrastructure.Middlewares;

public class JwtMiddleware
{
	public JwtMiddleware(RequestDelegate next, IOptions<ApplicationSettings> options) : base()
	{
		Next = next;
		ApplicationSettings = options.Value;
	}

	protected RequestDelegate Next { get; }
	protected ApplicationSettings ApplicationSettings { get; }

	public async Task InvokeAsync(HttpContext context, ITokenServices tokenUtility)
	{
		var requestHeaders =
			context.Request.Headers["Authorization"];

		var token =
			requestHeaders
			.FirstOrDefault()?
			.Split(" ")
			.Last();

		if (string.IsNullOrEmpty(token) == false)
			await tokenUtility.AttachUserToContextByToken
				(context: context, token: token, secretKey: ApplicationSettings.JwtSettings?.SecretKeyForToken!);

		await Next(context);
	}
}
