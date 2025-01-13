namespace Infrastructure.Settings;

public class ApplicationSettings
{
	public static readonly string KeyName = nameof(ApplicationSettings);

	public ApplicationSettings() : base()
	{
		DatabaseSetting = new DatabaseSetting();

		CorsSettings = new CorsSettings();
	}

	public required JwtSettings JwtSettings { get; set; }

	public required IdentitySettings IdentitySettings { get; set; }

	public DatabaseSetting DatabaseSetting { get; init; }

	public CorsSettings CorsSettings { get; init; }

	public int RequestBodyLimitSize { get; set; } = 30_000_000;

	public bool EnableSwagger { get; set; } = false;

	public bool EnableHangfire { get; set; } = false;

	public string ClientUrl { get; set; } = string.Empty;

	public bool EnableResponseCompression { get; set; }
}
