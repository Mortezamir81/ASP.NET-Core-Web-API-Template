namespace Services;

public abstract class BaseServices
{
	public BaseServices(IHttpContextAccessor httpContextAccessor)
	{
		HttpContextAccessor = httpContextAccessor;
	}


	public IHttpContextAccessor HttpContextAccessor { get; }


	public UserInformationInToken? GetUserFromContext()
	{
		var user =
			(HttpContextAccessor?.HttpContext?.Items["User"] as UserInformationInToken);

		return user;
	}


	public async Task<string> SaveFileAsync
		(IFormFile file, IHostEnvironment environment, string path, string? fileName = null)
	{
		var fileExtension =
				Path.GetExtension
					(path: file.FileName)?.ToLower();

		var rootPath =
			environment.ContentRootPath;

		string? newFileName = null;


		if (string.IsNullOrEmpty(fileName))
		{
			newFileName = $"{Guid.NewGuid()}{fileExtension}";
		}
		else
		{
			newFileName = $"{fileName}{fileExtension}";
		}

		var physicalPathName =
			Path.Combine
				(path1: rootPath, path2: "wwwroot", path3: path, path4: newFileName);

		using (var stream = File.Create(path: physicalPathName))
		{
			await file.CopyToAsync(target: stream);

			await stream.FlushAsync();

			stream.Close();
		}

		return $"{path}/{newFileName}";
	}
}
