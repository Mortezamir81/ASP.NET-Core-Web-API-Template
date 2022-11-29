namespace Infrastructure.Extentions;

public static class CustomJwtAuthentication
{
	public static void AddCustomJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
	{
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			if (string.IsNullOrWhiteSpace(jwtSettings?.SecretKeyForToken))
				throw new ArgumentNullException(paramName: nameof(jwtSettings.SecretKeyForToken));

			if (string.IsNullOrWhiteSpace(jwtSettings?.SecretKeyForEncryptionToken))
				throw new ArgumentNullException(paramName: nameof(jwtSettings.SecretKeyForEncryptionToken));

			var secretkey =
				Encoding.UTF8.GetBytes(jwtSettings.SecretKeyForToken);

			var encryptionkey =
				Encoding.UTF8.GetBytes(jwtSettings.SecretKeyForEncryptionToken);

			var validationParameters = new TokenValidationParameters
			{
				ClockSkew = TimeSpan.Zero,
				RequireSignedTokens = true,

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(secretkey),

				RequireExpirationTime = true,
				ValidateLifetime = true,

				ValidateIssuer = false,
				ValidateAudience = false,

				TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
			};

			options.SaveToken = true;
			options.TokenValidationParameters = validationParameters;
			options.Events = new JwtBearerEvents
			{
				OnTokenValidated = async context =>
				{
					var cache =
						context.HttpContext.RequestServices.GetService<IEasyCachingProvider>();

					var databaseContext =
						context.HttpContext.RequestServices.GetService<DatabaseContext>();

					if (cache == null || databaseContext == null)
					{
						context.Fail(nameof(HttpStatusCode.Unauthorized));
						return;
					}

					var userId =
						context.Principal?.Claims.FirstOrDefault
							(current => current.Type == ClaimTypes.NameIdentifier)?.Value;


					var securityStamp =
						context.Principal?.Claims.FirstOrDefault
							(current => current.Type == nameof(User.SecurityStamp))?.Value;

					if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(securityStamp))
					{
						context.Fail(nameof(HttpStatusCode.Unauthorized));
						return;
					}

					var userInCache =
						await cache.GetAsync<bool>($"userId-{userId}-exist");

					if (!userInCache.HasValue)
					{
						bool isExistSecurityStamp =
							await databaseContext.Users!
							.Where(current => current.Id.ToString() == userId)
							.Where(current => current.SecurityStamp.ToString() == securityStamp)
							.AnyAsync();

						if (isExistSecurityStamp == false)
						{
							context.Fail(nameof(HttpStatusCode.Unauthorized));

							return;
						}
					}

					await Task.CompletedTask;
				},
				OnChallenge = async context =>
				{
					context.HandleResponse();

					await CreateUnAuthorizeResult(context.Response);
				}
			};
		});
	}


	public static async Task CreateUnAuthorizeResult(HttpResponse response)
	{
		response.StatusCode = (int)HttpStatusCode.Unauthorized;

		var result = new Result();

		result.AddErrorMessage(nameof(HttpStatusCode.Unauthorized));

		await response.WriteAsJsonAsync(result);
	}
}
