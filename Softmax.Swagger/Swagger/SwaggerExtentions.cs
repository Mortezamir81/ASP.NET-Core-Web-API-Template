namespace Softmax.Swagger;

public static class SwaggerExtentions
{
	public static void AddCustomSwaggerGen(this IServiceCollection services, IConfiguration configuration)
	{
		var swaggerSettings = new SwaggerSettings();
		configuration.GetSection("SwaggerSettings").Bind(swaggerSettings);

		services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();

		services.AddSwaggerSchemaBuilder();

		services.AddSwaggerGen(options =>
		{
			options.EnableAnnotations();

			#region Include XML Docs
			if (swaggerSettings.EnableXmlDocs)
			{
				var xmlFiles =
					Directory.GetFiles
						(AppContext.BaseDirectory, "*.xml", searchOption: SearchOption.TopDirectoryOnly).ToList();

				xmlFiles.ForEach
					(xmlFile => options.IncludeXmlComments(filePath: xmlFile, includeControllerXmlComments: true));
			}
			#endregion /Include XML Docs

			#region Filters
			// Convert Enum To String Value And Show In The Schema Model Section
			if (swaggerSettings.EnableEnumSchema)
				options.SchemaFilter<EnumSchemaFilter>();

			//Set summary of action if not already set
			if (swaggerSettings.EnableDefaultActionDescription)
				options.OperationFilter<ApplySummariesOperationFilter>();

			//Add bad request sample data for actions
			options.OperationFilter<BadRequestResponsesOperationFilter>();

			//Add not found sample data for actions
			options.OperationFilter<NotFoundResponsesOperationFilter>();

			//Add (Lock icon) to actions that need authorization and add responses
			if (swaggerSettings.EnableAuthroizationResponsesAndIcon)
				options.OperationFilter<AuthorizeOperationFilter>(true, "OAuth2");
			#endregion /Filters

			#region Add OAuth Authentication
			if (swaggerSettings.EnableAuthroizationResponsesAndIcon &&
				!string.IsNullOrWhiteSpace(swaggerSettings.AuthenticationUrl))
			{
				options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.OAuth2,
					Flows = new OpenApiOAuthFlows()
					{
						Password = new OpenApiOAuthFlow()
						{
							TokenUrl = new Uri(swaggerSettings.AuthenticationUrl),
						},
					},

				});
			}
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
