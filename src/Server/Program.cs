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

builder.Services.AddCustomIdentity
	(builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(IdentitySettings)}").Get<IdentitySettings>());

builder.Services.AddCustomCORS();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCustomLogger();

builder.Services.AddAutoDetectedServices();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCustomEFSecondLevelCache();

builder.Services.AddCustomCaching();

builder.Services.AddCustomApiVersioning();

builder.Services.AddCustomController();

builder.Services.AddCustomJwtAuthentication
	(builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(JwtSettings)}").Get<JwtSettings>());

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
	app.UseSwaggerBasicAuthorization();
	app.UseCustomSwaggerAndUI();
}

app.UseCors("DevCorsPolicy");

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
