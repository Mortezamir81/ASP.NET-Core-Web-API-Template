namespace Infrastructure.Swagger;

public class EnumSchemaFilter : ISchemaFilter
{
	public void Apply(OpenApiSchema model, SchemaFilterContext context)
	{
		if (context.Type.IsEnum)
		{
			model.Enum.Clear();

			model.Example = new OpenApiInteger(1);

			Enum.GetNames(context.Type)
				.ToList()
				.ForEach(name => model.Enum.Add(new OpenApiString($"{Convert.ToInt64(Enum.Parse(context.Type, name))}: {name}")));
		}
	}
}
