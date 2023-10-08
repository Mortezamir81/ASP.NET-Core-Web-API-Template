namespace Infrastructure.Settings;

public class CorsSettings
{
	public bool Enable { get; set; }

	public string? AllowOrigins { get; set; }

	public string? AllowHeaders { get; set; }

	public string? AllowMethods { get; set; }
}
