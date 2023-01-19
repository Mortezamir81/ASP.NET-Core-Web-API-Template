using Microsoft.AspNetCore.Mvc;

namespace Dtat.Result
{
    public static class ResultExtensions
	{
		static ResultExtensions()
		{
		}


		public static ObjectResult ApiResult(this Result result)
		{
			ObjectResult objectResult = null;

			if (result.IsSuccess)
			{
				objectResult = new OkObjectResult(result);
			}

			switch (result.MessageCode)
			{
				case (int) MessageCodes.HttpBadRequestCode:
					objectResult = new BadRequestObjectResult(result);
					break;

				case (int) MessageCodes.HttpNotFoundError:
					objectResult = new NotFoundObjectResult(result);
					break;

				case (int) MessageCodes.HttpServerError:
					objectResult = new ObjectResult(result);
					objectResult.StatusCode = (int) MessageCodes.HttpServerError;
					break;
			}

			return objectResult;
		}
	}
}
