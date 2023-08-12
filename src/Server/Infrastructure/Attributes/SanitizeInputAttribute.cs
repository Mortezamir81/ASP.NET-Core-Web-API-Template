using Ganss.Xss;

namespace Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class SanitizeInputAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext filterContext)
	{
		if (filterContext is null)
		{
			return;
		}

		var sanitizer =
			filterContext.HttpContext.RequestServices.GetRequiredService<IHtmlSanitizer>();

		filterContext.CleanupActionStringValues(data => sanitizer.Sanitize(data));
	}
}