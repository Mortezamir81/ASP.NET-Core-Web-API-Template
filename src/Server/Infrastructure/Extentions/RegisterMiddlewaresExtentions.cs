namespace Infrastructure.Extentions;

public static class RegisterMiddlewaresExtentions
{
	public static void RegisterMiddlewares(this WebApplication app, ApplicationSettings? applicationSettings)
	{
		if (applicationSettings == null)
		{
			throw new ArgumentNullException(nameof(applicationSettings));
		}

		if (app.Environment.IsProduction())
		{
			app.UseGlobalExceptionMiddleware();
		}
		else
		{
			app.UseDeveloperExceptionPage();
		}

		if (applicationSettings.EnableSwagger == true)
		{
			app.UseSwaggerBasicAuthorization();
			app.UseCustomSwaggerAndUI(uiOptions: new CustomSwaggerUiOptions
			{
				CustomJsPathes = new List<string>
				{
					"/js/swagger/sort.js",
					"/js/swagger/swagger-utils.js"
				}
			});
		}

		app.UseCors("DevCorsPolicy");

		app.UseStaticFiles(new StaticFileOptions()
		{
			OnPrepareResponse = ctx =>
			{
				ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
				ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
				  "Origin, X-Requested-With, Content-Type, Accept");
			},

		});

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();
	}
}
