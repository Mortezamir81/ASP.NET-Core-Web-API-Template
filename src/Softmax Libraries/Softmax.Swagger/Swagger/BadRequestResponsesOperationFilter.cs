﻿namespace Softmax.Swagger;

public class BadRequestResponsesOperationFilter : IOperationFilter
{
	private readonly IOpenApiObjectBuilder _objectBuilder;

	public BadRequestResponsesOperationFilter(IOpenApiObjectBuilder objectBuilder)
	{
		_objectBuilder = objectBuilder;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var badRequsetResult = new Result<object>();

		badRequsetResult.AddErrorMessage
			(nameof(MessageCode.HttpBadRequestCode), messageCode: MessageCode.HttpBadRequestCode);

		var badRequestResponse =
			new Dictionary<string, OpenApiMediaType>
			{
					{
						"application/json",
						new OpenApiMediaType
						{
							Example = _objectBuilder.Build(badRequsetResult)
						}
					}
			};

		operation.Responses.TryAdd("400", new OpenApiResponse
		{
			Description = "BadRequest",
			Content = badRequestResponse
		});
	}
}
