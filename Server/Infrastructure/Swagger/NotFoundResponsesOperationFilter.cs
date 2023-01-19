using Dtat.Result;

namespace Infrastructure.Swagger;

public class NotFoundResponsesOperationFilter : IOperationFilter
{
	private readonly IOpenApiObjectBuilder _objectBuilder;

	public NotFoundResponsesOperationFilter(IOpenApiObjectBuilder objectBuilder)
	{
		_objectBuilder = objectBuilder;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var badRequsetResult = new Result<object>();

		badRequsetResult.AddErrorMessage
			(nameof(MessageCodes.HttpNotFoundError), messageCodes: MessageCodes.HttpNotFoundError);

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

		operation.Responses.TryAdd("404", new OpenApiResponse
		{
			Description = "Not Found",
			Content = badRequestResponse
		});
	}
}
