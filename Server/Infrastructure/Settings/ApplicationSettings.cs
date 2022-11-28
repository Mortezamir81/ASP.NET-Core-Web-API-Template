namespace Infrastructure.Settings;

public class ApplicationSettings
{
    public static readonly string KeyName = nameof(ApplicationSettings);

    public ApplicationSettings() : base()
    {
    }

    public JwtSettings? JwtSettings { get; set; }
	}
