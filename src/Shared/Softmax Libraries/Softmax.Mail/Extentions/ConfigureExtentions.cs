using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Softmax.Mail.Settings;

namespace Softmax.Mail.Extentions;

public static class ConfigureExtentions
{
	public static void AddSofmaxMailServices
		(this IServiceCollection services, IConfiguration configuration)
	{
		var mailSettings = new MailSettings();

		configuration.GetSection("SoftmaxMailSettings").Bind(mailSettings);

		services.AddTransient<IEmailServices, EmailServices>(current => new EmailServices(mailSettings));
	}
}
