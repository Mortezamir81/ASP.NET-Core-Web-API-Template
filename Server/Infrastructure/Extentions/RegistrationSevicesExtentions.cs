using Dtat.Logging.NLog;
using System.Text.Json.Serialization;

namespace Infrastructure.Extentions;

public static class RegistrationSevicesExtentions
{
	public static void AddScopedServices(this IServiceCollection services)
	{
		services
			.AddScoped<IUserServices, UserServices>()
			.AddScoped<IUserRepository, UserRepository>()
			.AddScoped<IUnitOfWork, UnitOfWork>()
			.AddScoped<ITokenServices, TokenServices>()
			.AddScoped<LogInputParameterAttribute>();
	}


	public static void AddCustomLogger(this IServiceCollection services)
	{
		services.AddTransient<ILogger, NLogAdapter>();

		services.AddTransient
			(serviceType: typeof(ILogger<>),
				implementationType: typeof(NLogAdapter<>));
	}


	public static void AddCustomDbContext(this IServiceCollection services, string? connectionString)
	{
		Assert.NotEmpty(obj: connectionString, name: nameof(connectionString));

		services.AddDbContextPool<DatabaseContext>(option =>
		{
			option.UseSqlServer(connectionString: connectionString);
		});
	}


	public static void AddCustomController(this IServiceCollection services)
	{
		services
			.AddControllers(options =>
			{
				options.Filters.Add<ValidationAttribute>();
				options.Filters.Add<CustomExceptionHandlerAttribute>();
				options.Filters.Add(new LogInputParameterAttribute(InputLogLevel.Debug));
			})
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			})
			.ConfigureApiBehaviorOptions(options =>
			{
				options.SuppressModelStateInvalidFilter = true;
			});
	}


	public static void AddCustomCORS(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy("DevCorsPolicy", builder =>
			{
				builder
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			});
		});
	}


	public static void AddCustomCaching(this IServiceCollection services)
	{
		services.AddEasyCaching(options =>
		{
			options.UseInMemory();
		});
	}


	public static void AddCustomApiVersioning(this IServiceCollection services)
	{
		services.AddApiVersioning(options =>
		{
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
			options.ReportApiVersions = true;
		});

		services.AddVersionedApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});
	}


	public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings? settings)
	{
		Assert.NotNull(obj: services, name: nameof(services));

		services.Configure<SecurityStampValidatorOptions>(option =>
		{
			option.ValidationInterval = TimeSpan.FromSeconds(1);
		});

		services.AddIdentity<User, Role>(identityOptions =>
		{
			//Password Settings
			identityOptions.Password.RequireDigit = settings!.PasswordRequireDigit;
			identityOptions.Password.RequiredLength = settings!.PasswordRequiredLength;
			identityOptions.Password.RequireNonAlphanumeric = settings!.PasswordRequireNonAlphanumic;
			identityOptions.Password.RequireUppercase = settings!.PasswordRequireUppercase;
			identityOptions.Password.RequireLowercase = settings!.PasswordRequireLowercase;

			//UserName Settings
			identityOptions.User.RequireUniqueEmail = settings!.RequireUniqueEmail;

			//Singin Settings
			//identityOptions.SignIn.RequireConfirmedEmail = false;
			//identityOptions.SignIn.RequireConfirmedPhoneNumber = false;

			//Lockout Settings
			//identityOptions.Lockout.MaxFailedAccessAttempts = 5;
			//identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
			//identityOptions.Lockout.AllowedForNewUsers = false;
		})
		.AddEntityFrameworkStores<DatabaseContext>()
		.AddDefaultTokenProviders();
	}
}
