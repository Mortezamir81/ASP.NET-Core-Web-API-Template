using Dtat.Result;

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
				(exception: context.Exception,
					message: context.Exception.Message,
					methodName: actionName, classType: context.Controller.GetType());

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
