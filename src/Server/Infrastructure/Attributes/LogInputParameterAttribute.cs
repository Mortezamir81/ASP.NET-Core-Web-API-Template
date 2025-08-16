namespace Infrastructure.Attributes;

public class LogInputParameterAttribute : ActionFilterAttribute, IRegisterAsScoped
{
	private readonly InputLogLevel _inputLogLevel;

	public LogInputParameterAttribute(InputLogLevel inputLogLevel = InputLogLevel.Information)
	{
		_inputLogLevel = inputLogLevel;
	}

	public override void OnActionExecuting(ActionExecutingContext filterContext)
	{
		var actionArguments = filterContext.ActionArguments;

		var logger =
			filterContext.HttpContext.RequestServices.GetService<ILogger>();

		if (actionArguments?.Count > 0)
		{
			var requestViewModel =
				actionArguments.ElementAt(0).Value;

			if (requestViewModel != null)
			{
				var actionName =
					filterContext.RouteData.Values["action"]?.ToString();

				var parameters = new List<object?>
				{
					requestViewModel,
				};

				switch (_inputLogLevel)
				{
					case InputLogLevel.Debug:
						logger?.LogDebug
							(message: Resources.Resource.InputPropertiesInfo,
								args: new { parameters },
								classType: filterContext.Controller.GetType());
						break;

					case InputLogLevel.Information:
						logger?.LogInformation
							(message: Resources.Resource.InputPropertiesInfo,
								args: new { parameters },
								classType: filterContext.Controller.GetType());
						break;

					case InputLogLevel.Warning:
						logger?.LogWarning
							(message: Resources.Resource.InputPropertiesInfo,
								args: new { parameters },
								classType: filterContext.Controller.GetType());
						break;
				}
			}
		}

		base.OnActionExecuting(filterContext);
	}
}
