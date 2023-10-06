namespace Softmax.Mail.Settings;

public class MailSettings
{
	public string? From { get; set; }

	public string? SmtpHost { get; set; }
	public int? SmtpPort { get; set; }
	public int? SmtpTimeout { get; set; }

	public string? SmtpUser { get; set; }
	public string? SmtpPass { get; set; }


	public string? BccAddresses { get; set; }

	public string? EmailSubjectTemplate { get; set; }
}
