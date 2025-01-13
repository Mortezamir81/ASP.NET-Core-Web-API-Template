namespace Services;

public class TokenServices : ITokenServices, IRegisterAsScoped
{
	#region MainMethods
	/// <summary>
	/// Generate jwt access token
	/// </summary>
	public string GenerateJwtToken
		(string securityKey, ClaimsIdentity claimsIdentity, DateTimeOffset dateTime)
	{
		Assert.NotEmpty(securityKey, nameof(securityKey));

		var signingCredentional =
			GenerateSigningCredential(securityKey!);

		var tokenDescriptor =
			GenerateTokenDescriptor(claimsIdentity, dateTime.UtcDateTime, signingCredentional);

		var tokenHandler = new JwtSecurityTokenHandler();

		SecurityToken securityToken =
			tokenHandler.CreateToken(tokenDescriptor);

		string token =
			tokenHandler.WriteToken(securityToken);

		return token;
	}
	#endregion /MainMethods

	#region SubMethods
	private SigningCredentials GenerateSigningCredential(string securityKey)
	{
		byte[] key =
			Encoding.ASCII.GetBytes(securityKey);

		var symmetricSecurityKey =
			new SymmetricSecurityKey(key: key);

		var securityAlgorithm =
			SecurityAlgorithms.HmacSha256Signature;

		var signingCredential =
			new SigningCredentials(key: symmetricSecurityKey, algorithm: securityAlgorithm);

		return signingCredential;
	}


	private SecurityTokenDescriptor GenerateTokenDescriptor
		(ClaimsIdentity claimsIdentity, DateTime dateTime, SigningCredentials signingCredential)
	{
		var tokenDescriptor =
			new SecurityTokenDescriptor()
			{
				Subject = claimsIdentity,

				//Issuer = "",
				//Audience = "",

				Expires = dateTime,
				SigningCredentials = signingCredential,
			};

		return tokenDescriptor;
	}
	#endregion /SubMethods
}
