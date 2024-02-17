using Hangfire;
using Infrastructure.Hangfire;

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

			app.UseHsts();
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

		if (applicationSettings.CorsSettings.Enable)
		{
			app.UseCors("DevCorsPolicy");
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles(new StaticFileOptions()
		{
			OnPrepareResponse = ctx =>
			{
				// Cache static files for 30 days
				ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");

				ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");

				ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
					"Origin, X-Requested-With, Content-Type, Accept");
			},
		});

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		if (app.Environment.IsProduction())
		{
			app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				Authorization = new[] { new AuthorizationHangfireFilter() }
			});
		}
		else
		{
			app.UseHangfireDashboard();
		}

		if (applicationSettings.EnableResponseCompression)
		{
			app.UseResponseCompression();
		}

		app.MapControllers();

		app.MapHangfireDashboard();
	}
}
