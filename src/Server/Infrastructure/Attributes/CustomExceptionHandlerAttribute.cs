using Dtat.Logging;

namespace Infrastructure.Attributes;

public class CustomExceptionHandlerAttribute : ActionFilterAttribute
{
	private readonly ILogger _logger;

	public CustomExceptionHandlerAttribute(ILogger logger)
	{
		_logger = logger;
	}

	public override void OnActionExecuted(ActionExecutedContext context)
	{
		if (context.Exception != null)
		{
			var actionName =
				context.RouteData.Values["action"]?.ToString();

			_logger.LogCritical
				(ex: context.Exception,
					type: context.Controller.GetType(),
					message: context.Exception.Message,
					arg1: new { ActionName = actionName, Handler = "CustomExceptionHandler" });

			var result = new Result();

			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.UnkonwnError);

			result.AddErrorMessage(message: errorMessage, messageCode: MessageCode.HttpServerError);

			context.Result = result.ApiResult();

			context.ExceptionHandled = true;
		}

		base.OnActionExecuted(context);
	}
}
