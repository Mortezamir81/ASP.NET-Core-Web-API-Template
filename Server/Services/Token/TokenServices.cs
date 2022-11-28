namespace Services;

public class TokenServices : ITokenServices
{
	#region Constractor
	public TokenServices
		(ILogger<TokenServices> logger,
		DatabaseContext databaseContext,
		IEasyCachingProvider cache)
	{
		Cache = cache;
		Logger = logger;
		DatabaseContext = databaseContext;
	}
	#endregion /Constractor

	#region Properties
	public IEasyCachingProvider Cache { get; }
	private ILogger<TokenServices> Logger { get; }
	public DatabaseContext DatabaseContext { get; }
	#endregion /Properties

	#region MainMethods
	public async Task AttachUserToContextByToken
		(HttpContext context, string token, string secretKey)
	{
		try
		{
			var key =
				Encoding.ASCII.GetBytes(secretKey);

			var tokenHandler =
				new JwtSecurityTokenHandler();

			tokenHandler.ValidateToken(token: token,
				validationParameters: new TokenValidationParameters()
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateIssuerSigningKey = true,

					RequireExpirationTime = true,
					ValidateLifetime = true,

					IssuerSigningKey =
						new SymmetricSecurityKey(key: key),

					ClockSkew =
						TimeSpan.Zero
				}, out SecurityToken validatedToken);

			var jwtToken =
				validatedToken as JwtSecurityToken;

			if (jwtToken == null)
				return;

			var securityStampClaim =
				jwtToken.Claims
				.Where(current => current.Type.ToLower() == nameof(User.SecurityStamp).ToLower())
				.FirstOrDefault();

			var roleIdClaim =
				jwtToken.Claims
				.Where(current => current.Type.ToLower() == nameof(User.RoleId).ToLower())
				.FirstOrDefault();

			var userIdClaim =
				jwtToken.Claims
				.Where(current => current.Type.ToLower() == nameof(User.Id).ToLower())
				.FirstOrDefault();


			if (securityStampClaim == null)
				return;

			if (userIdClaim == null)
				return;

			if (roleIdClaim == null)
				return;

			if (!Guid.TryParse(securityStampClaim.Value, out var securityStamp))
				return;

			if (!long.TryParse(userIdClaim.Value, out var userId))
				return;

			if (!int.TryParse(roleIdClaim.Value, out var roleId))
				return;

			var userInCache = 
				await Cache.GetAsync<bool>($"userId-{userId}-exist");

			if (!userInCache.HasValue)
			{
				bool isExistSecurityStamp =
					await CheckUserSecurityStampAsync(securityStamp, userId);

				if (isExistSecurityStamp == false)
					return;
			}

			var userInformationInToken = new UserInformationInToken()
			{
				Id = userId,
				RoleId = roleId,
			};

			context.Items["User"] = userInformationInToken;
		}
		catch (Exception ex)
		{
			if (ex.Message.Contains("Lifetime"))
			{
				return;
			}

			string errorMessage = string.Format
				(Resources.Messages.ErrorMessages.InvalidJwtToken);

			Logger.LogError(ex, errorMessage);
		}
	}


	public string GenerateJwtToken
		(string securityKey, ClaimsIdentity claimsIdentity, DateTime dateTime)
	{
		var signingCredentional =
			GetSigningCredentional(securityKey);

		var tokenDescriptor =
			GetTokenDescriptor(claimsIdentity, dateTime, signingCredentional);

		var tokenHandler = new JwtSecurityTokenHandler();

		SecurityToken securityToken =
			tokenHandler.CreateToken(tokenDescriptor);

		string token =
			tokenHandler.WriteToken(securityToken);

		return token;
	}
	#endregion /MainMethods

	#region SubMethods
	private SigningCredentials GetSigningCredentional(string securityKey)
	{
		byte[] key =
			Encoding.ASCII.GetBytes(securityKey);

		var symmetricSecurityKey =
			new SymmetricSecurityKey(key: key);

		var securityAlgorithm =
			SecurityAlgorithms.HmacSha256Signature;

		var signingCredentional =
			new SigningCredentials(key: symmetricSecurityKey, algorithm: securityAlgorithm);

		return signingCredentional;
	}


	private async Task<bool> CheckUserSecurityStampAsync(Guid securityStamp, long userId)
	{
		var result =
			await DatabaseContext.Users!
				.AsNoTracking()
				.Select(current => new { current.Id, current.SecurityStamp })
				.Where(current => current.SecurityStamp == securityStamp && current.Id == userId)
				.AnyAsync()
				;

		return result;
	}


	private SecurityTokenDescriptor GetTokenDescriptor
		(ClaimsIdentity claimsIdentity, DateTime dateTime, SigningCredentials signingCredentional)
	{
		var tokenDescriptor =
			new SecurityTokenDescriptor()
			{
				Subject = claimsIdentity,

				//Issuer = "",
				//Audience = "",

				Expires = dateTime,
				SigningCredentials = signingCredentional
			};

		return tokenDescriptor;
	}
	#endregion /SubMethods
}
