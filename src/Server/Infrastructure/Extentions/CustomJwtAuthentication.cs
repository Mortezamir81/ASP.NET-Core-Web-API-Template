namespace Infrastructure.Extentions;

public static class CustomJwtAuthentication
{
	public static void AddCustomJwtAuthentication(this IServiceCollection services, JwtSettings? jwtSettings)
	{
		Assert.NotNull(obj: services, name: nameof(services));

		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

					TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey),
				};

				options.SaveToken = true;
				options.TokenValidationParameters = validationParameters;

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = async context =>
					{
						string? access_token =
							context.Request.Cookies["access_token"] ?? context.Request.Cookies["token"];

						if (!string.IsNullOrEmpty(access_token))
						{
							context.Token = access_token;

							return;
						}

						access_token = context.Request.Query["access_token"];

						if (!string.IsNullOrEmpty(access_token))
						{
							context.Token = access_token;

							return;
						}

						access_token = context.Request.Query["token"];

						if (!string.IsNullOrEmpty(access_token))
						{
							context.Token = access_token;

							return;
						}

						context.Token = access_token;

						await Task.CompletedTask;
					},
					OnTokenValidated = async context =>
					{
						var userTokenIdRaw =
							context.Principal?.Claims.FirstOrDefault
							(c => c.Type == Constants.Authentication.UserTokenId)?.Value;

						var userId =
							context.Principal?.GetUserId();

						if (!userId.HasValue ||
							string.IsNullOrWhiteSpace(userTokenIdRaw) ||
							!int.TryParse(userTokenIdRaw, out var userTokenId))
						{
							context.Fail(nameof(HttpStatusCode.Unauthorized));
							return;
						}

						var cache =
							context.HttpContext.RequestServices.GetRequiredService<IEasyCachingProvider>();

						var userInCache =
							await cache.GetAsync<bool>($"user-Id-{userId}-logged-in");

						if (userInCache.HasValue)
							return;

						var databaseContext =
							context.HttpContext.RequestServices.GetRequiredService<DatabaseContext>();

						var userToken =
							await databaseContext.UserAccessTokens!
							.FirstOrDefaultAsync(current => current.Id == userTokenId);

						var accessToken =
							context.SecurityToken as JwtSecurityToken;

						if (userToken == null || userToken.AccessToken != accessToken?.RawData)
						{
							context.Fail(nameof(HttpStatusCode.Unauthorized));
							return;
						}

						await cache.TrySetAsync
							($"user-Id-{userId}-logged-in", true, TimeSpan.FromHours(jwtSettings.UserTimeInCache));

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
		response.StatusCode = (int) HttpStatusCode.Unauthorized;

		var result = new Result();

		result.AddErrorMessage(nameof(HttpStatusCode.Unauthorized));

		await response.WriteAsJsonAsync(result);
	}
}
