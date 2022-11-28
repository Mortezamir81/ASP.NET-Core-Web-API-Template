namespace Infrastructure.Swagger;

public class UnauthorizedResponsesOperationFilter : IOperationFilter
{
	private readonly string _schemeName;
	private readonly IOpenApiObjectBuilder _objectBuilder;
	private readonly bool _includeUnauthorizedAndForbiddenResponses;

	public UnauthorizedResponsesOperationFilter
		(string schemeName,
		IOpenApiObjectBuilder objectBuilder,
		bool includeUnauthorizedAndForbiddenResponses)
	{
		_includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
		_objectBuilder = objectBuilder;
		_schemeName = schemeName;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var filters =
			context.ApiDescription.ActionDescriptor.FilterDescriptors;

		var hasAnonymous =
			filters.Any(p => p.Filter is AllowAnonymousFilter);

		if (hasAnonymous)
			return;

		var hasAuthorize =
			filters.Any(p => p.Filter is Infrastructure.Attributes.AuthorizeAttribute);

		if (!hasAuthorize)
			return;

		if (_includeUnauthorizedAndForbiddenResponses)
		{
			#region Unauthorized Response
			var unauthorizedResult = new Result();

			unauthorizedResult.AddErrorMessage("Unauthorized");

			var unauthorizedResponse =
				new Dictionary<string, OpenApiMediaType>
				{
					{
						"application/json",
						new OpenApiMediaType
						{
							Example = _objectBuilder.Build(unauthorizedResult)
						}
					}
				};

			operation.Responses.TryAdd("401", new OpenApiResponse 
			{ 
				Description = "Unauthorized", Content = unauthorizedResponse 
			});
			#endregion /Unauthorized Response

			#region Forbidden Response
			var forbiddenResult = new Result();

			forbiddenResult.AddErrorMessage("Forbidden");

			var forbiddenResponse =
				new Dictionary<string, OpenApiMediaType>
				{
					{
						"application/json",
						new OpenApiMediaType
						{
							Example = _objectBuilder.Build(forbiddenResult)
						}
					}
				};

			operation.Responses.TryAdd("403", new OpenApiResponse
			{ 
				Description = "Forbidden", Content = forbiddenResponse
			});
			#endregion /Forbidden Response
		}

		operation.Security = new List<OpenApiSecurityRequirement>()
		{
			new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = _schemeName
						}
					},
					new string[] {}
				}
			}
		};
	}
}
