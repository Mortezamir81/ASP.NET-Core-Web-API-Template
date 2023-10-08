namespace Infrastructure.Settings;

public class JwtSettings
{
	public JwtSettings() : base()
	{
	}

	public int UserTimeInCache { get; set; }

	public required string SecretKeyForToken { get; set; }

	public int AccessTokenExpiresPerHour { get; set; }

	public int RefreshTokenExpiresPerDay { get; set; }

	public required string SecretKeyForEncryptionToken { get; set; }
}
