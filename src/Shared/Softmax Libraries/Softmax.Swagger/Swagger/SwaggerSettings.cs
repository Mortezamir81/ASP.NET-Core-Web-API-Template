namespace Softmax.Swagger;

public class SwaggerSettings
{
	public bool UseHttpsForAuth { get; set; } = true;

	public string? AuthenticationUrl { get; set; }

	public bool EnableEnumSchema { get; set; }

	public bool EnableXmlDocs { get; set; }

	public bool EnableDefaultActionDescription { get; set; }

	public bool EnableAuthroizationResponsesAndIcon { get; set; }
}
