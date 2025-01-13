namespace Softmax.Swagger;

public class NotFoundResponsesOperationFilter : IOperationFilter
{
	private readonly IOpenApiObjectBuilder _objectBuilder;

	public NotFoundResponsesOperationFilter(IOpenApiObjectBuilder objectBuilder)
	{
		_objectBuilder = objectBuilder;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var badRequestResult = new Result<object>();

		badRequestResult.AddErrorMessage
			(message: nameof(MessageCode.HttpNotFoundError), MessageCode.HttpNotFoundError);

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

		operation.Responses.TryAdd("404", new OpenApiResponse
		{
			Description = "Not Found",
			Content = badRequestResponse
		});
	}
}
