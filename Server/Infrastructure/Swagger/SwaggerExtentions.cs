namespace Infrastructure.Swagger;

public static class SwaggerExtentions
{
	public static void AddCustomSwaggerGen(this IServiceCollection services)
	{
		services.AddSwaggerSchemaBuilder();

		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });

			options.EnableAnnotations();

			#region Include XML Docs
			var currentProjectName =
				Assembly.GetExecutingAssembly().FullName?.Split(',')?[0];

			var xmlDocPath =
				Path.Combine(AppContext.BaseDirectory, $"{currentProjectName}.xml");

			options.IncludeXmlComments
				(filePath: xmlDocPath, includeControllerXmlComments: true);
			#endregion /Include XML Docs

			#region Filters
			// Convert Enum To String Value And Show In The Schema Model Section
			options.SchemaFilter<EnumSchemaFilter>();

			//Set summary of action if not already set
			options.OperationFilter<ApplySummariesOperationFilter>();

			//Add (Lock icon) to actions that need authorization and add responses
			options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "OAuth2");
			#endregion /Filters

			#region Add OAuth Authentication
			options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.OAuth2,
				Flows = new OpenApiOAuthFlows()
				{
					Password = new OpenApiOAuthFlow()
					{
						TokenUrl = new Uri("https://localhost:7200/api/users/LoginByOAuth"),
					},
				},
				
			});
			#endregion
		});
	}

	public static void UseCustomSwaggerAndUI(this IApplicationBuilder app)
	{
		app.UseSwagger();

		app.UseSwaggerUI(options =>
		{
			#region Customizing
			options.DisplayRequestDuration();
			options.DocExpansion(DocExpansion.None);
			#endregion

			options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
		});
	}
}
