using Hangfire.Dashboard;

namespace Infrastructure.Hangfire;

public class AuthorizationHangfireFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context)
	{
		var httpContext = context.GetHttpContext();

		if (httpContext.User.Identity == null || httpContext.User.Identity?.IsAuthenticated == false)
			return false;

		if (!httpContext.User.IsInRole(Constants.Role.SystemAdmin))
			return false;

		return true;
	}
}