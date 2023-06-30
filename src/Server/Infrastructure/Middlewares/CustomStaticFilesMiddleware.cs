namespace Infrastructure.Middlewares;

public class CustomStaticFilesMiddleware : object
{
	public CustomStaticFilesMiddleware
		(RequestDelegate next, IHostEnvironment hostEnvironment) : base()
	{
		Next = next;
		HostEnvironment = hostEnvironment;
	}

	private RequestDelegate Next { get; }

	private IHostEnvironment HostEnvironment { get; }

	public async Task InvokeAsync(HttpContext httpContext)
	{
		string requestPath =
			httpContext.Request.Path;

		if (string.IsNullOrWhiteSpace(requestPath) || requestPath == "/")
		{
			await Next(httpContext);
			return;
		}

		if (requestPath.StartsWith("/") == false)
		{
			await Next(httpContext);
			return;
		}

		requestPath =
			requestPath[1..];


		var rootPath =
			HostEnvironment.ContentRootPath;

		var physicalPathName =
			Path.Combine
				(path1: rootPath, path2: "wwwroot", path3: requestPath);


		if (File.Exists(physicalPathName) == false)
		{
			await Next(httpContext);
			return;
		}

		var fileExtension =
			Path.GetExtension(physicalPathName)?.ToLower();

		switch (fileExtension)
		{		
			case ".jpg":
			case ".jpeg":
			{
				httpContext.Response.StatusCode = 200;
				httpContext.Response.ContentType = "image/jpeg";

				break;
			}

			case ".png":
			{
				httpContext.Response.StatusCode = 200;
				httpContext.Response.ContentType = "image/png";

				break;
			}

			default:
			{
				await Next(httpContext);
				return;
			}
		}

		await httpContext.Response
			.SendFileAsync(fileName: physicalPathName);
	}
}