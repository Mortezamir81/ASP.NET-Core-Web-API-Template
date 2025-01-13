using Microsoft.AspNetCore.Mvc.Filters;

namespace Softmax.Swagger;

public class AuthorizeOperationFilter : IOperationFilter
{
	private readonly string _schemeName;
	private readonly IOpenApiObjectBuilder _objectBuilder;
	private readonly bool _includeUnauthorizedAndForbiddenResponses;

	public AuthorizeOperationFilter
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
		var hasAllowAnonymous = context.MethodInfo
			.GetCustomAttributes(true)
			.OfType<AllowAnonymousAttribute>()
			.Any();

		if (hasAllowAnonymous)
			return;

		var hasAuthorize = context.MethodInfo
			.GetCustomAttributes(true)
			.OfType<AuthorizeAttribute>()
			.Any();

		var hasAuthorizeApi = context.MethodInfo
			.GetCustomAttributes(true)
			.OfType<IAsyncAuthorizationFilter>()
			.Any();

		var hasOptionalAuthorize = context.MethodInfo
			.GetCustomAttributes(true)
			.OfType<Softmax.Swagger.Attributes.OptionalAuthorizeAttribute>()
			.Any();

		if (!hasAuthorize && !hasOptionalAuthorize && !hasAuthorizeApi)
			return;

		if (_includeUnauthorizedAndForbiddenResponses)
		{
			#region Unauthorized Response
			var unauthorizedResult = new Result();

			unauthorizedResult.AddErrorMessage
				(message: nameof(MessageCode.HttpUnauthorizeError), MessageCode.HttpUnauthorizeError);

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
				Description = "Unauthorized",
				Content = unauthorizedResponse
			});
			#endregion /Unauthorized Response

			#region Forbidden Response
			var forbiddenResult = new Result();

			forbiddenResult.AddErrorMessage
				(message: nameof(MessageCode.HttpForbidenError), MessageCode.HttpForbidenError);

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
				Description = "Forbidden",
				Content = forbiddenResponse
			});
			#endregion /Forbidden Response
		}

		operation.Security =
		[
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
					Array.Empty<string>()
				}
			}
		];
	}
}
