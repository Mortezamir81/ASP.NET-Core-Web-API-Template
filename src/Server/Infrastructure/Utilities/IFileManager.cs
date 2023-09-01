namespace Infrastructure.Utilities;

public interface IFileManager
{
	Task<SaveFileResult> SaveFileAsync
		(IFormFile file, string path, string fileName, bool includeWebRootPath = false, string? root = null);


	Task<SaveFileResult> SaveFileWithRandomeNameAsync
		(IFormFile file, string path, bool includeWebRootPath = false, string? root = null);


	Task<SaveFileResult> SaveStreamWithRandomeNameAsync
		(Stream stream, string fileExtension, string path, bool includeWebRootPath = false, string? root = null);


	bool DeleteFile(string? path, bool includeWebRootPath = false, string? root = null);


	public string? GetFileExtention(string fileName);
}
