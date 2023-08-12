namespace Services;

public interface ITokenServices
{
	string GenerateJwtToken
		(string securityKey, ClaimsIdentity claimsIdentity, DateTimeOffset dateTime);
}
