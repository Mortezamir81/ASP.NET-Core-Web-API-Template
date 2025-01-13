namespace Softmax.Swagger;

public class BadRequestResponsesOperationFilter : IOperationFilter
{
	private readonly IOpenApiObjectBuilder _objectBuilder;

	public BadRequestResponsesOperationFilter(IOpenApiObjectBuilder objectBuilder)
	{
		_objectBuilder = objectBuilder;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var badRequestResult = new Result<object>();

		badRequestResult.AddErrorMessage
			(message: nameof(MessageCode.HttpBadRequestCode), MessageCode.HttpBadRequestCode);

		var badRequestResponse =
			new Dictionary<string, OpenApiMediaType>
			{
					{
						"application/json",
						new OpenApiMediaType
						{
							Example = _objectBuilder.Build(badRequestResult)
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
