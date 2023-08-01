namespace Softmax.Swagger.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class OptionalAuthorizeAttribute : Attribute
{
	public OptionalAuthorizeAttribute() : base()
	{

	}
}
