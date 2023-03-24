namespace Softmax.Swagger;

public class SwaggerConfigureOptions : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;

	public SwaggerConfigureOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

	public void Configure(SwaggerGenOptions options)
	{
		foreach (var desc in _provider.ApiVersionDescriptions)
		{
			options.SwaggerDoc(desc.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
			{
				Title = "My Test API",
				Version = desc.ApiVersion.ToString(),
			});
		}
	}
}
