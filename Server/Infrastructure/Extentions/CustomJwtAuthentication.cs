namespace Infrastructure.Extentions;

public static class CustomJwtAuthentication
{
	public static void AddCustomJwtAuthentication(this IServiceCollection services, JwtSettings? jwtSettings)
	{
		Assert.NotNull(obj: services, name: nameof(services));

		services
			.AddAuthentication()
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
