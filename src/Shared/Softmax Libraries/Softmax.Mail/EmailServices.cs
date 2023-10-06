namespace Softmax.Mail;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Softmax.Mail.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

public interface IEmailServices
{
	Task SendAsync
		(string[] to,
		string subject,
		string body,
		MessagePriority priority = MessagePriority.High,
		MailSettings? mailSettings = null,
		CancellationToken cancellationToken = default);
}

public class EmailServices : IEmailServices
{
	private readonly MailSettings? _settings;

	public EmailServices(MailSettings? options = null)
	{
		_settings = options;
	}

	public async Task SendAsync
		(string[] to,
		string subject,
		string body,
		MessagePriority priority = MessagePriority.High,
		MailSettings? mailSettings = null,
		CancellationToken cancellationToken = default)
	{
		//********************
		CheckRequirements
			(to: to, subject: subject, body: body, methodSettings: mailSettings);
		//********************

		//********************
		var email = new MimeMessage()
		{
			Subject = subject,
			Body = new TextPart(TextFormat.Html)
			{
				Text = body
			},
			Priority = priority switch
			{
				MessagePriority.Low => MimeKit.MessagePriority.NonUrgent,
				MessagePriority.Normal => MimeKit.MessagePriority.Normal,
				MessagePriority.High => MimeKit.MessagePriority.Urgent,
				_ => MimeKit.MessagePriority.Urgent,
			},
		};

		var from =
			_settings?.From ?? mailSettings?.From;

		var user =
			_settings?.SmtpUser ?? mailSettings?.SmtpUser;

		email.From.Add(new MailboxAddress(from, user));

		foreach (var toItem in to)
			email.To.Add(new MailboxAddress(toItem, toItem));

		//********************

		//********************
		using var smtp = new SmtpClient();

		var host =
			_settings?.SmtpHost ?? mailSettings?.SmtpHost;

		var port =
			_settings?.SmtpPort ?? mailSettings?.SmtpPort ?? 0;

		var pass = _settings?.SmtpPass ?? mailSettings?.SmtpPass;

		await smtp.ConnectAsync
			(host, port, SecureSocketOptions.Auto, cancellationToken: cancellationToken);

		await smtp.AuthenticateAsync(user, pass, cancellationToken: cancellationToken);

		await smtp.SendAsync(email, cancellationToken: cancellationToken);

		await smtp.DisconnectAsync(true, cancellationToken: cancellationToken);
		//********************
	}

	public void CheckRequirements
		(string[] to, string subject, string body, MailSettings? methodSettings)
	{
		if (string.IsNullOrWhiteSpace(body))
		{
			throw new ArgumentNullException(nameof(body));
		}

		if (string.IsNullOrWhiteSpace(subject))
		{
			throw new ArgumentNullException(nameof(subject));
		}

		if (to == null || to.Length == 0)
		{
			throw new ArgumentNullException(nameof(to));
		}

		if (string.IsNullOrWhiteSpace(_settings?.From) && string.IsNullOrWhiteSpace(methodSettings?.From))
		{
			throw new ArgumentNullException(nameof(_settings.From));
		}

		if (string.IsNullOrWhiteSpace(_settings?.SmtpHost) && string.IsNullOrWhiteSpace(methodSettings?.SmtpHost))
		{
			throw new ArgumentNullException
				(nameof(_settings.SmtpHost), "You have not configured SmtpHost in appSettings.json or method parameter.");
		}

		if (string.IsNullOrWhiteSpace(_settings?.SmtpUser) && string.IsNullOrWhiteSpace(methodSettings?.SmtpUser))
		{
			throw new ArgumentNullException
				(nameof(_settings.SmtpHost), "You have not configured SmtpUser in appSettings.json or method parameter.");
		}

		if (string.IsNullOrWhiteSpace(_settings?.SmtpPass) && string.IsNullOrWhiteSpace(methodSettings?.SmtpPass))
		{
			throw new ArgumentNullException
				(nameof(_settings.SmtpHost), "You have not configured SmtpPass in appSettings.json or method parameter.");
		}

		if (_settings?.SmtpPort == null && methodSettings?.SmtpPort == null)
		{
			throw new ArgumentNullException
				(nameof(_settings.SmtpHost), "You have not configured SmtpPass in appSettings.json or method parameter.");
		}
	}
}

public enum MessagePriority
{
	Low = 0,
	Normal = 1,
	High = 2
}

