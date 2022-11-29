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
