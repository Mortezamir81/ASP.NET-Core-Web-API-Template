//******************************
using Infrastructure.Extentions;

var builder =
	WebApplication.CreateBuilder(args);
//******************************


#region Services
//******************************
builder.Services.Configure<ApplicationSettings>
	(builder.Configuration.GetSection(ApplicationSettings.KeyName));

builder.Services.AddCustomDbContext
	(connectionString: builder.Configuration.GetConnectionString("MySqlServerConnectionString"));

builder.Services.AddCustomIdentity
	(builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(IdentitySettings)}").Get<IdentitySettings>());

builder.Services.AddCustomCORS();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCustomLogger();

builder.Services.AddScopedServices();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCustomCaching();

builder.Services.AddCustomApiVersioning();

builder.Services.AddCustomController();

builder.Services.AddCustomJwtAuthentication
	(builder.Configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(JwtSettings)}").Get<JwtSettings>());

builder.Services.AddCustomSwaggerGen();
//******************************
#endregion /Services


//******************************
var app =
	builder.Build();
//******************************


#region Middlewares
//******************************
await app.IntializeDatabase();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseGlobalExceptionMiddleware();

	app.UseSwaggerBasicAuthorization();
}

app.UseCustomSwaggerAndUI();

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
