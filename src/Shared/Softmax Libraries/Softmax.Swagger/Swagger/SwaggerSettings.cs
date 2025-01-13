namespace Softmax.Swagger;

public class SwaggerSettings
{
	public bool EnableEnumSchema { get; set; }

	public bool EnableXmlDocs { get; set; }

	public bool EnableDefaultActionDescription { get; set; }

	public bool EnableAuthorizationResponsesAndIcon { get; set; }

	public string? SchemaIdSelector { get; set; }

	public string? Title { get; set; } = "Swagger API";

	public string? Description { get; set; }
}

public class CustomSwaggerUiOptions
{
	public List<string>? CustomJsPathes { get; set; }
}
