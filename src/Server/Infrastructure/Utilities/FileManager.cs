namespace Infrastructure.Utilities;

public class FileManager : IFileManager, IRegisterAsSingleton
{
	#region Fields
	private readonly IHostEnvironment _hostEnvironment;
	#endregion /Fields

	#region Constractor
	public FileManager
		(IHostEnvironment hostEnvironment,
		ILogger<FileManager> logger)
	{
		_hostEnvironment = hostEnvironment;
	}
	#endregion /Constractor

	#region Public Methods
	public async Task<SaveFileResult> SaveFileAsync
		(IFormFile file, string path, string fileName, bool includeRootPath = false, string? root = null)
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

		if (!string.IsNullOrWhiteSpace(root))
		{
			finalPathName =
				Path.Combine
					(path1: root, path2: path, path3: newFileName);
		}
		else
		{
			finalPathName =
				Path.Combine
					(path1: path, path2: newFileName);
		}

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
				ContentType = file.ContentType,
				Extension = fileExtension,
				FileName = newFileName,
				ByteSize = file.Length,
				SavedUrl = Path.Combine(path, newFileName)
			};

		return result;
	}


	public async Task<SaveFileResult> SaveFileWithRandomeNameAsync
		(IFormFile file, string path, bool includeRootPath = false, string? root = null)
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

		if (!string.IsNullOrWhiteSpace(root))
		{
			finalPathName =
				Path.Combine
					(path1: root, path2: path, path3: newFileName);
		}
		else
		{
			finalPathName =
				Path.Combine
					(path1: path, path2: newFileName);
		}

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
				ContentType = file.ContentType,
				Extension = fileExtension,
				FileName = newFileName,
				ByteSize = file.Length,
				SavedUrl = Path.Combine(path, newFileName)
			};

		return result;
	}


	public async Task<SaveFileResult> SaveStreamWithRandomeNameAsync
		(Stream stream, string fileExtension, string path, bool includeRootPath = false, string? root = null)
	{
		string newFileName =
			$"{Guid.NewGuid()}.{fileExtension}";

		string finalPathName;

		if (!string.IsNullOrWhiteSpace(root))
		{
			finalPathName =
				Path.Combine
					(path1: root, path2: path, path3: newFileName);
		}
		else
		{
			finalPathName =
				Path.Combine
					(path1: path, path2: newFileName);
		}

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

		await CopyStreamToFileAsync(fileStream: stream, path: finalPathName);

		var result =
			new SaveFileResult()
			{
				Extension = fileExtension,
				FileName = newFileName,
				ByteSize = stream.Length,
				SavedUrl = Path.Combine(path, newFileName)
			};

		return result;
	}


	public bool DeleteFile(string? path, bool includeRootPath = false, string? root = null)
	{
		if (string.IsNullOrWhiteSpace(path))
			return false;

		var finalPath = path;

		if (!string.IsNullOrWhiteSpace(root))
		{
			finalPath =
				Path.Combine(path1: root, path2: path);
		}

		if (includeRootPath)
		{
			var rootPath =
				_hostEnvironment.ContentRootPath;

			finalPath =
				Path.Combine(path1: rootPath, path2: finalPath);
		}

		var isFileExist =
			File.Exists(finalPath);

		if (!isFileExist)
			return false;

		File.Delete(finalPath);

		return true;
	}


	public string? GetFileExtention(string fileName)
	{
		var fileExtension =
			Path.GetExtension
				(path: fileName)?.ToLower();

		return fileExtension;
	}
	#endregion Public Methods

	#region Private Methods
	private async Task CopyFileToAsync(IFormFile file, string path)
	{
		using var stream = File.Create(path: path);

		await file.CopyToAsync(target: stream);

		await stream.FlushAsync();

		stream.Close();
	}

	private async Task CopyStreamToFileAsync(Stream fileStream, string path)
	{
		using var stream = File.Create(path: path);

		fileStream.Seek(0, SeekOrigin.Begin);

		await fileStream.CopyToAsync(stream);

		await stream.FlushAsync();

		stream.Close();
	}
	#endregion Private Methods
}

public class SaveFileResult
{
	public string? ContentType { get; set; }

	public required string FileName { get; set; }

	public required string Extension { get; set; }

	public required string SavedUrl { get; set; }

	public long ByteSize { get; set; }
}
