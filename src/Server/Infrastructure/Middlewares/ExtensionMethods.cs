namespace Infrastructure.Middlewares;

public static class ExtensionMethods
{
	static ExtensionMethods()
	{
	}

	public static IApplicationBuilder
		UseGlobalExceptionMiddleware(this IApplicationBuilder app)

	{
		return app.UseMiddleware<GlobalExceptionMiddleware>();
	}

	public static IApplicationBuilder
		UseCustomStaticFilesMiddleware(this IApplicationBuilder app)

	{
		return app.UseMiddleware<CustomStaticFilesMiddleware>();
	}
}
