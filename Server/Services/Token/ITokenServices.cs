namespace Services;

public interface ITokenServices
{
	Task AttachUserToContextByToken
		(HttpContext context, string token, string secretKey);

	string GenerateJwtToken
		(string securityKey, ClaimsIdentity claimsIdentity, DateTime dateTime);
}
