namespace Infrastructure.Swagger;

public class EnumSchemaFilter : ISchemaFilter
{
	public void Apply(OpenApiSchema model, SchemaFilterContext context)
	{
		if (context.Type.IsEnum)
		{
			model.Enum.Clear();

			Enum.GetNames(context.Type)
				.ToList()
				.ForEach(name => model.Enum.Add(new OpenApiString($"{name}")));
		}
	}
}
