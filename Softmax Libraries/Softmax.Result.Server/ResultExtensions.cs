using Microsoft.AspNetCore.Mvc;

namespace Dtat.Results.Server
{
	public static class ResultExtensions
	{
		static ResultExtensions()
		{
		}


		public static ObjectResult ApiResult(this Result result)
		{
			if (result.IsFailed)
			{
				return new BadRequestObjectResult(result);
			}

			return new OkObjectResult(result);
		}
	}
}
