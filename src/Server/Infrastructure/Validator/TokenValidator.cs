using Infrastructure.Utilities;

namespace Infrastructure.Validator;

public class TokenValidator : ITokenValidator
{
	public async Task ExecuteAsync(TokenValidatedContext context, JwtSettings jwtSettings)
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

		if (userToken == null || userToken.AccessTokenHash != SecuriyHelper.ToSha256(accessToken?.RawData))
		{
			context.Fail(nameof(HttpStatusCode.Unauthorized));
			return;
		}

		await cache.TrySetAsync
			($"user-Id-{userId}-logged-in", true, TimeSpan.FromHours(jwtSettings.UserTimeInCache));

		await Task.CompletedTask;
	}
}
