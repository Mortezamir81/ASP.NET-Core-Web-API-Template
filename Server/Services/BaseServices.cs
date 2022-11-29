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
		var userContext = 
			HttpContextAccessor?.HttpContext?.User;

		if (userContext?.Identity?.IsAuthenticated == true) 
		{
			var stringUserId =
				userContext?.Claims.FirstOrDefault(current => current.Type == ClaimTypes.NameIdentifier)?.Value;

			var stringRoleId =
				userContext?.Claims.FirstOrDefault(current => current.Type == nameof(User.RoleId))?.Value;

			if (stringUserId == null || stringRoleId == null)
				return default;

			var userId = long.Parse(stringUserId);
			var roleId = int.Parse(stringRoleId);

			var user =
				new UserInformationInToken
				{
					Id = userId,
					RoleId = roleId,
				};

			return user;
		}

		return default;
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
