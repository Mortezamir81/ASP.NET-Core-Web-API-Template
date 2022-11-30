namespace Infrastructure.Settings;

public class ApplicationSettings
{
    public static readonly string KeyName = nameof(ApplicationSettings);

    public ApplicationSettings() : base()
    {
    }

    public required JwtSettings JwtSettings { get; set; }
}
