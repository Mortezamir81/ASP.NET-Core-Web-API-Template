namespace Softmax.Swagger;

public class SwaggerSettings
{
	public bool EnableEnumSchema { get; set; }

	public bool EnableXmlDocs { get; set; }

	public bool EnableDefaultActionDescription { get; set; }

	public bool EnableAuthroizationResponsesAndIcon { get; set; }
}

public class CustomSwaggerUiOptions
{
	public List<string>? CustomJsPathes { get; set; }
}
