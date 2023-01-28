namespace Services;

public class TokenServices : ITokenServices
{
	#region MainMethods
	/// <summary>
	/// Generate jwt access token
	/// </summary>
	/// <param name="securityKey"></param>
	/// <param name="claimsIdentity"></param>
	/// <param name="dateTime"></param>
	/// <returns></returns>
	public string GenerateJwtToken
		(string securityKey, ClaimsIdentity claimsIdentity, DateTime dateTime)
	{
		Assert.NotEmpty(securityKey, nameof(securityKey));

		var signingCredentional =
			GenerateSigningCredentional(securityKey!);

		var tokenDescriptor =
			GenerateTokenDescriptor(claimsIdentity, dateTime, signingCredentional);

		var tokenHandler = new JwtSecurityTokenHandler();

		SecurityToken securityToken =
			tokenHandler.CreateToken(tokenDescriptor);

		string token =
			tokenHandler.WriteToken(securityToken);

		return token;
	}
	#endregion /MainMethods

	#region SubMethods
	private SigningCredentials GenerateSigningCredentional(string securityKey)
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


	private SecurityTokenDescriptor GenerateTokenDescriptor
		(ClaimsIdentity claimsIdentity, DateTime dateTime, SigningCredentials signingCredentional)
	{
		var tokenDescriptor =
			new SecurityTokenDescriptor()
			{
				Subject = claimsIdentity,

				//Issuer = "",
				//Audience = "",

				Expires = dateTime,
				SigningCredentials = signingCredentional,
			};

		return tokenDescriptor;
	}
	#endregion /SubMethods
}
