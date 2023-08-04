using EFCoreSecondLevelCacheInterceptor;
using Infrastructure.Utilities;

namespace Infrastructure.Extentions;

public static class RegistrationSevicesExtentions
{
	public static void AddAutoDetectedServices(this IServiceCollection services)
	{
		services.Scan(scan => scan
		//Register Scoped Services
		.FromApplicationDependencies()
		.AddClasses(classes => classes.AssignableTo<IRegisterAsScoped>())
		.AsImplementedInterfaces()
		.AsSelf()
		.WithScopedLifetime()

		//Register Transient Services
		.AddClasses(classes => classes.AssignableTo<IRegisterAsTransient>())
		.AsImplementedInterfaces()
		.AsSelf()
		.WithTransientLifetime()

		//Register Singleton Services
		.AddClasses(classes => classes.AssignableTo<IRegisterAsSingleton>())
		.AsImplementedInterfaces()
		.AsSelf()
		.WithSingletonLifetime());
	}


	public static void AddRecursiveEntityUtilites(this IServiceCollection services)
	{
		services.AddTransient
			(serviceType: typeof(IRecursiveEntityUtilites<>),
				implementationType: typeof(RecursiveEntityUtilites<>));
	}


	public static void AddCustomEFSecondLevelCache(this IServiceCollection services)
	{
		services.AddEFSecondLevelCache(options =>
		{
			options
				.UseMemoryCacheProvider()
				.DisableLogging(false)
				.UseCacheKeyPrefix("EF_")
				.SkipCachingCommands(commandText => commandText.Contains("NEWID()", StringComparison.InvariantCultureIgnoreCase))
				.SkipCachingResults(result => result.Value == null || (result.Value is EFTableRows rows && rows.RowsCount == 0));

			options.CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30));
		});

	}


	public static void AddCustomLogger(this IServiceCollection services)
	{
		services.AddSingleton<ILogger, NLogAdapter>();

		services.AddSingleton
			(serviceType: typeof(ILogger<>),
				implementationType: typeof(NLogAdapter<>));
	}


	public static void AddCustomDbContext(this IServiceCollection services, DatabaseSetting databaseSetting)
	{
		Assert.NotEmpty(obj: databaseSetting, name: nameof(databaseSetting));

		switch (databaseSetting.DatabaseProviderType)
		{
			case Settings.Enums.DatabaseProviderType.SQLite:
				services.AddDbContextPool<DatabaseContext>((serviceProvider, optionsBuilder) =>
				{
					optionsBuilder
						.UseSqlite(databaseSetting.SQLiteConnectionString, sqliteOptionsAction: current =>
						{
							current.MigrationsAssembly
								(assemblyName: "Persistence.SQLite");
						})
						.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
				});
				break;

			case Settings.Enums.DatabaseProviderType.SqlServer:
				services.AddDbContextPool<DatabaseContext>((serviceProvider, optionsBuilder) =>
				{
					optionsBuilder
						.UseSqlServer(databaseSetting.SqlServerConnectionString, sqlServerOptionsAction: current =>
						{
							current.MigrationsAssembly
								(assemblyName: "Persistence.SqlServer");
						})
						.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
				});
				break;

			case Settings.Enums.DatabaseProviderType.PostgreSql:
				services.AddDbContextPool<DatabaseContext>((serviceProvider, optionsBuilder) =>
				{
					optionsBuilder
						.UseNpgsql(databaseSetting.PostgreSqlConnectionString, npgsqlOptionsAction: current =>
						{
							current.MigrationsAssembly
								(assemblyName: "Persistence.PostgreSql");
						})
						.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
				});
				break;
		}
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
