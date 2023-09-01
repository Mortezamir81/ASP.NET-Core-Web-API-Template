using Microsoft.Extensions.Logging;

//******************************
var builder =
	WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
	builder.Logging.AddConsole();

builder.Configuration.AddEnvironmentVariables();
//******************************


#region Services
//******************************
var applicationSettings =
	builder.Configuration.GetSection
	(ApplicationSettings.KeyName).Get<ApplicationSettings>();

builder.Services.Configure<ApplicationSettings>
	(builder.Configuration.GetSection(ApplicationSettings.KeyName));

builder.Services.AddMemoryCacheService();

builder.Services.AddCustomDbContext(applicationSettings!.DatabaseSetting);

builder.Services.AddCustomIdentity(applicationSettings!.IdentitySettings);

builder.Services.AddCustomCORS();

builder.Services.AddCustomLogger();

builder.Services.AddAutoDetectedServices();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCustomEFSecondLevelCache();

builder.Services.AddCustomCaching();

builder.Services.AddCustomApiVersioning();

builder.Services.AddCustomController();

builder.Services.AddRequestBodySizeLimit(applicationSettings.RequestBodyLimitSize);

builder.Services.AddCustomJwtAuthentication(applicationSettings.JwtSettings);

builder.Services.AddCustomSwaggerGen(builder.Configuration);
//******************************
#endregion /Services


//******************************
var app =
	builder.Build();
//******************************


#region Middlewares
//******************************
await app.IntializeDatabaseAsync();

if (app.Environment.IsProduction())
{
	app.UseGlobalExceptionMiddleware();
}
else
{
	app.UseDeveloperExceptionPage();
}

if (applicationSettings.EnableSwagger == true)
{
	app.UseSwaggerBasicAuthorization();
	app.UseCustomSwaggerAndUI(uiOptions: new CustomSwaggerUiOptions
	{
		CustomJsPath = "/js/swagger/sort.js"
	});
}

app.UseCors("DevCorsPolicy");

app.UseStaticFiles(new StaticFileOptions()
{
	OnPrepareResponse = ctx =>
	{
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
		ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
		  "Origin, X-Requested-With, Content-Type, Accept");
	},

});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//******************************
#endregion /Middlewares


//******************************
app.Run();
//******************************

public partial class Program { }
