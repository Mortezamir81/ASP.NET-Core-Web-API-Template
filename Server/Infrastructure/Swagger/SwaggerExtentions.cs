namespace Infrastructure.Swagger;

public static class SwaggerExtentions
{
	public static void AddCustomSwaggerGen(this IServiceCollection services)
	{
		services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();

		services.AddSwaggerSchemaBuilder();

		services.AddSwaggerGen(options =>
		{
			options.EnableAnnotations();

			#region Include XML Docs
			var currentProjectName =
				Assembly.GetExecutingAssembly().FullName?.Split(',')?[0];

			var xmlDocPath =
				Path.Combine(AppContext.BaseDirectory, $"{currentProjectName}.xml");

			var xmlDocPathForSharedProject =
				Path.Combine(AppContext.BaseDirectory, $"Shared.xml");

			options.IncludeXmlComments
				(filePath: xmlDocPathForSharedProject);

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

		var apiVersionDescriptionProvider =
			app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

		app.UseSwaggerUI(options =>
		{
			#region Customizing
			options.DisplayRequestDuration();
			options.DocExpansion(DocExpansion.None);
			#endregion

			if (apiVersionDescriptionProvider == null)
				return;

			foreach (var desc in apiVersionDescriptionProvider.ApiVersionDescriptions)
			{
				options.SwaggerEndpoint($"../swagger/{desc.GroupName}/swagger.json", $"V{desc.ApiVersion.MajorVersion}");
			}
		});
	}
}
