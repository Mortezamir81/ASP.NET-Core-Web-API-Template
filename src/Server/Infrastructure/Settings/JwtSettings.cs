namespace Infrastructure.Settings;

public class JwtSettings
{
	public JwtSettings() : base()
	{
	}

	public required string SecretKeyForToken { get; set; }

	public required int TokenExpiresTime { get; set; }

	public required string SecretKeyForEncryptionToken { get; set; }
}
