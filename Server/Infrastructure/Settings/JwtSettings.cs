namespace Infrastructure.Settings;

public class JwtSettings
{
	public JwtSettings() : base()
	{
	}

	public string? SecretKeyForToken { get; set; }

	public int? TokenExpiresTime { get; set; }
}
