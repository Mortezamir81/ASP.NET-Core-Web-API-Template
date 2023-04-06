namespace Infrastructure.Utilities;

public class FileManager : IFileManager
{
	#region Fields
	private readonly IHostEnvironment _hostEnvironment;
	private readonly ILogger<FileManager> _logger;
	#endregion /Fields

	#region Constractor
	public FileManager
		(IHostEnvironment hostEnvironment,
		ILogger<FileManager> logger)
	{
		_hostEnvironment = hostEnvironment;
		_logger = logger;
	}
	#endregion /Constractor

	#region Public Methods
	public async Task<SaveFileResult> SaveFileAsync
		(IFormFile file, string path, string fileName, bool includeRootPath = false)
	{
		var fileExtension =
				GetFileExtention(file.FileName);

		if (string.IsNullOrWhiteSpace(fileExtension))
		{
			throw new ArgumentNullException(nameof(fileExtension));
		}

		string newFileName =
			$"{fileName}{fileExtension}";

		string finalPathName;

		finalPathName =
			Path.Combine
				(path1: path, path2: newFileName);

		if (includeRootPath)
		{
			var rootPath =
				_hostEnvironment.ContentRootPath;

			finalPathName =
				Path.Combine(path1: rootPath, path2: finalPathName);
		}

		var finalPath =
			Path.GetDirectoryName(finalPathName);

		if (!Directory.Exists(finalPath))
		{
			Directory.CreateDirectory(finalPath!);
		}

		await CopyFileToAsync(file: file, path: finalPathName);

		var result =
			new SaveFileResult()
			{
				Extention = fileExtension,
				FileName = newFileName,
				ByteSize = file.Length,
				SavedUrl = $"{path}/{newFileName}"
			};

		return result;
	}


	public async Task<SaveFileResult> SaveFileWithRandomeNameAsync
		(IFormFile file, string path, bool includeRootPath = false)
	{
		var fileExtension =
			GetFileExtention(file.FileName);

		if (string.IsNullOrWhiteSpace(fileExtension))
		{
			throw new ArgumentNullException(nameof(fileExtension));
		}

		string newFileName =
			$"{Guid.NewGuid()}{fileExtension}";

		string finalPathName;

		finalPathName =
			Path.Combine
				(path1: path, path2: newFileName);

		if (includeRootPath)
		{
			var rootPath =
				_hostEnvironment.ContentRootPath;

			finalPathName =
				Path.Combine(path1: rootPath, path2: finalPathName);
		}

		var finalPath =
			Path.GetDirectoryName(finalPathName);

		if (!Directory.Exists(finalPath))
		{
			Directory.CreateDirectory(finalPath!);
		}

		await CopyFileToAsync(file: file, path: finalPathName);

		var result =
			new SaveFileResult()
			{
				Extention = fileExtension,
				FileName = newFileName,
				ByteSize = file.Length,
				SavedUrl = $"{path}/{newFileName}"
			};

		return result;
	}


	public bool DeleteFile(string? path, bool includeRootPath = false)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(path))
				return false;

			var finalPath = path;

			if (includeRootPath)
			{
				var rootPath =
					_hostEnvironment.ContentRootPath;

				finalPath =
					Path.Combine(path1: rootPath, path2: "wwwroot", path3: path);
			}

			var isFileExist =
				File.Exists(finalPath);

			if (!isFileExist)
				return false;

			File.Delete(finalPath);

			return true;
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex);

			return false;
		}
	}
	#endregion Public Methods

	#region Private Methods
	private string? GetFileExtention(string fileName)
	{
		var fileExtension =
			Path.GetExtension
				(path: fileName)?.ToLower();

		return fileExtension;
	}


	private async Task CopyFileToAsync(IFormFile file, string path)
	{
		using (var stream = File.Create(path: path))
		{
			await file.CopyToAsync(target: stream);

			await stream.FlushAsync();

			stream.Close();
		}
	}
	#endregion Private Methods
}

public class SaveFileResult
{
	public required string FileName { get; set; }

	public required string Extention { get; set; }

	public required string SavedUrl { get; set; }

	public long ByteSize { get; set; }
}
