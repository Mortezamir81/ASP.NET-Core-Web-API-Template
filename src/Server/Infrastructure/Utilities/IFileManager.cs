using Microsoft.Extensions.Hosting;

namespace Infrastructure.Utilities;

public interface IFileManager
{
	Task<SaveFileResult> SaveFileAsync
		(IFormFile file, string path, string fileName, bool includeRootPath = false);

	Task<SaveFileResult> SaveFileWithRandomeNameAsync
		(IFormFile file, string path, bool includeRootPath = false);

	bool DeleteFile(string? path, bool includeRootPath = false);
}
