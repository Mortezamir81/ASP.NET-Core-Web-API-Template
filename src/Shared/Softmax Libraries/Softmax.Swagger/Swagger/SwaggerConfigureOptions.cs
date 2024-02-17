namespace Softmax.Swagger;

public class SwaggerConfigureOptions : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;
	private readonly SwaggerSettings _swaggerSettings;

	public SwaggerConfigureOptions(IApiVersionDescriptionProvider provider, IOptions<SwaggerSettings> swaggerSettings)
	{
		_provider = provider;
		_swaggerSettings = swaggerSettings.Value;
	}

	public void Configure(SwaggerGenOptions options)
	{
		foreach (var desc in _provider.ApiVersionDescriptions)
		{
			options.SwaggerDoc(desc.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
			{
				Title = _swaggerSettings.Title,
				Version = desc.ApiVersion.ToString(),
				Description = _swaggerSettings.Description,
			});
		}
	}
}
