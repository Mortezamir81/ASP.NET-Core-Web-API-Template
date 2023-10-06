using Hangfire;
using Softmax.Mail;
using System.Threading;

namespace Services;

public partial class EmailSenderWithSchedule
{
	#region Fields
	[AutoInject] private readonly IEmailServices _emailServices;
	[AutoInject] private readonly IBackgroundJobClient _backgroundJobClient;
	[AutoInject] private readonly ILogger<EmailSenderWithSchedule> _logger;
	#endregion /Fields

	#region Public Methods 
	public void SendWithSchedule(string to, string subject, string body)
	{
		_backgroundJobClient.Enqueue(() => SendAsync(to, subject, body));
	}

	public async Task SendAsync(string to, string subject, string body)
	{
		try
		{
			await _emailServices.SendAsync(to: new[] { to },
				subject: subject,
				body: body,
				priority: MessagePriority.High,
				mailSettings: null,
				cancellationToken: CancellationToken.None);
		}
		catch (Exception e)
		{
			_logger.LogCritical(exception: e, message: e.Message, parameters: new List<object?>
			{
				to, subject, body
			});

			throw;
		}
	}
	#endregion /Public Methods 
}
