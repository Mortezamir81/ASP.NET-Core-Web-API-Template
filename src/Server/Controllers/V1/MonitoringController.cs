using EFCoreSecondLevelCacheInterceptor;
using System.Diagnostics;

namespace Server.Controllers.V1;

/// <summary>
/// Monitoring the resource usage in application
/// </summary>
public partial class MonitoringController : BaseController
{
	#region Fields
	[AutoInject] private readonly IEasyCachingProvider _cache;
	[AutoInject] private readonly IEFCacheServiceProvider _efCacheServiceProvider;
	#endregion /Fields

	#region Constractor
	#endregion Constractor

	#region HttpGet
	/// <summary>
	/// Get memory information from current process
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

	#region HttpDelete
	/// <summary>
	/// Remove all caches
	/// </summary>
	[HttpDelete("Caches")]
	[Authorize(Roles = $"{Constants.Role.SystemAdmin}")]
	public async Task<ActionResult> RemoveAllCaches()
	{
		_efCacheServiceProvider.ClearAllCachedEntries();

		await _cache.RemoveByPrefixAsync($"user-Id");

		return Ok();
	}
	#endregion /HttpGet
}