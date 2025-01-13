using Microsoft.Extensions.Logging;

//******************************
var builder =
	WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
	builder.Logging.AddConsole();

builder.Configuration.AddEnvironmentVariables();
//******************************


//******************************
var applicationSettings =
	builder.Configuration.GetSection
	(ApplicationSettings.KeyName).Get<ApplicationSettings>();
//******************************


#region Services
//******************************
builder.Services.RegisterServices
	(configuration: builder.Configuration, applicationSettings: applicationSettings);
//******************************
#endregion /Services


//******************************
var app =
	builder.Build();
//******************************


#region Middlewares
//******************************
await app.InitializeDatabaseAsync();

app.RegisterMiddleware(applicationSettings: applicationSettings);
//******************************
#endregion /Middlewares


//******************************
app.Run();
//******************************

public partial class Program { }
