namespace Services;

public class EmailTemplateService : object
{
	#region Static Methods

	#region GetFolderName()
	private static string GetFolderName()
	{
		var result =
			$"email_templates";

		return result;
	}
	#endregion /GetFolderName()

	#region CreateEmptyFile()
	private static void CreateEmptyFile(string pathName)
	{
		try
		{
			System.IO.File.Create(path: pathName);
		}
		catch { }
	}
	#endregion /CreateEmptyFile()

	#region GetContentByFileNameAsync()
	public static async System.Threading.Tasks.Task<string?>
		GetContentByFileNameAsync(string physicalRootPath, string fileName)
	{
		var currentUICultureName =
			Domain.Common.CultureEnumHelper.GetCurrentUICultureName();

		var pathName =
			$"{physicalRootPath}\\{GetFolderName()}\\{fileName}.{currentUICultureName}.html";

		if (System.IO.File.Exists(path: pathName) == false)
		{
			CreateEmptyFile(pathName: pathName);

			return null;
		}

		var content =
			await
			System.IO.File.ReadAllTextAsync(path: pathName);

		return content;
	}
	#endregion /GetContentByFileNameAsync()

	#endregion /Static Methods

	#region Constructor
	public EmailTemplateService(Microsoft.Extensions
		.Hosting.IHostEnvironment hostEnvironment) : base()
	{
		HostEnvironment = hostEnvironment;
	}
	#endregion /Constructor

	#region Properties

	public Microsoft.Extensions.Hosting.IHostEnvironment HostEnvironment { get; init; }

	#endregion /Properties

	#region Read Only Properties

	#region private string PhysicalRootPath { get; }
	private string PhysicalRootPath
	{
		get
		{
			var result =
				$"{HostEnvironment.ContentRootPath}\\wwwroot";

			return result;
		}
	}
	#endregion /private string PhysicalRootPath { get; }

	#endregion /Read Only Properties

	#region Methods

	#region GetContentForNewUserAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForNewUserAsync()
	{
		var fileName = "new-user";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForNewUserAsync()

	#region GetContentForContactingAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForContactingAsync()
	{
		var fileName =
			"contacting";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForContactingAsync()

	#region GetContentForRegistrationAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForRegistrationAsync()
	{
		var fileName =
			"registration";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForRegistrationAsync()

	#region GetContentForUnexpectedErrorAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForUnexpectedErrorAsync()
	{
		var fileName =
			"unexpected-error";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForUnexpectedErrorAsync()

	#region GetContentForModifyingOldPostAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForModifyingOldPostAsync()
	{
		var fileName = "update-post";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForModifyingOldPostAsync()

	#region GetContentForPublishingNewPostAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForPublishingNewPostAsync()
	{
		var fileName = "new-post";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForPublishingNewPostAsync()

	#region GetContentForResettingPasswordAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForResettingPasswordAsync()
	{
		var fileName =
			"resetting-password";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForResettingPasswordAsync()

	#region GetContentForCreatingOrUpdatingPostAsync()
	public async System.Threading.Tasks.Task<string?> GetContentForCreatingOrUpdatingPostAsync()
	{
		var fileName =
			"creating-or-updating-post";

		var content =
			await
			GetContentByFileNameAsync
			(physicalRootPath: PhysicalRootPath, fileName: fileName);

		return content;
	}
	#endregion /GetContentForCreatingOrUpdatingPostAsync()

	#endregion /Methods
}
