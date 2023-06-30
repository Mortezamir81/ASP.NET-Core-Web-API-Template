using System.Diagnostics;

namespace Server.Controllers.V1;

/// <summary>
/// Monitoring the resource usage in application
/// </summary>
public class MonitoringController : BaseController
{
	#region Fields
	#endregion /Fields

	#region Constractor
	#endregion Constractor

	#region HttpGet
	/// <summary>
	/// Get momeory information from current process
	/// </summary>
	[HttpGet("GetMemoryUsage")]
	[Authorize(Roles = $"{Constants.Role.SystemAdmin}")]
	public ActionResult GetMemoryUsage()
	{
		var currentProcess =
			Process.GetCurrentProcess();

		var physicalMemoryUsageInMB =
			currentProcess.WorkingSet64 / (1024 * 1024);

		return Ok($"{physicalMemoryUsageInMB} MB");
	}
	#endregion /HttpGet
}