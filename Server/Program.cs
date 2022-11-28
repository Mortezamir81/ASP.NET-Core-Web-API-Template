//******************************
var builder =
	WebApplication.CreateBuilder(args);
//******************************


#region Services
//******************************
builder.Services.AddCustomSwaggerGen();

builder.Services.Configure<ApplicationSettings>
	(builder.Configuration.GetSection(ApplicationSettings.KeyName));

builder.Services.AddDbContextPool<DatabaseContext>(option =>
{
	option.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("MySqlServerConnectionString"));
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("DevCorsPolicy", builder =>
	{
		builder
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient
	(serviceType: typeof(Dtat.Logging.ILogger<>),
		implementationType: typeof(Dtat.Logging.NLog.NLogAdapter<>));

builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddScoped<ITokenServices, TokenServices>();

builder.Services.AddScoped<LogInputParameterAttribute>();

builder.Services.AddAutoMapper(typeof(Infrastructure.AutoMapperProfiles.UserProfile));

builder.Services.AddControllers(config =>
{
	config.Filters.Add<ValidationAttribute>();
	config.Filters.Add<CustomExceptionHandlerAttribute>();
	config.Filters.Add(new LogInputParameterAttribute(InputLogLevel.Debug));
})
.ConfigureApiBehaviorOptions(options =>
{
	options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEasyCaching(options =>
{
	options.UseInMemory();
});

builder.Services.AddApiVersioning(options =>
{
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
	options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});
//******************************
#endregion /Services


//******************************
var app =
	builder.Build();
//******************************


#region Middlewares
//******************************
if (app.Environment.IsProduction())
{
	app.UseGlobalExceptionMiddleware();
}

app.UseCors("DevCorsPolicy");

app.UseCustomJwtMiddleware();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseCustomSwaggerAndUI();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});
//******************************
#endregion /Middlewares


//******************************
app.Run();
//******************************

public partial class Program { }
