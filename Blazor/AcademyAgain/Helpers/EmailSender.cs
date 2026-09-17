using AcademyAgain.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AcademyAgain.Helpers
{
	public interface IEmailSender
	{
		Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
	}

	public class EmailSender(IOptions<SmtpOptions> options, ILogger<EmailSender> logger) : IEmailSender
	{
		private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);
		private readonly SmtpOptions _opt = options.Value;

		public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
		{
			if (!_opt.IsConfigured)
			{
				logger.LogWarning("SMTP не настроен — письмо для {To} не отправлено (тема: {Subject})", to, subject);
				return;
			}

			if (!MailboxAddress.TryParse(_opt.From, out var from))
			{
				throw new InvalidOperationException($"Некорректный адрес отправителя SMTP: «{_opt.From}».");
			}
			from.Name = _opt.FromName;

			if (!MailboxAddress.TryParse(to, out var recipient))
			{
				throw new InvalidOperationException($"Некорректный адрес получателя: «{to}».");
			}

			var message = new MimeMessage();
			message.From.Add(from);
			message.To.Add(recipient);
			message.Subject = subject;
			message.Body = new TextPart("html") { Text = body };

			using var client = new SmtpClient { Timeout = (int)Timeout.TotalMilliseconds };
			var socketOptions = _opt.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;

			try
			{
				await client.ConnectAsync(_opt.Host, _opt.Port, socketOptions, ct);
				if (!string.IsNullOrWhiteSpace(_opt.User))
				{
					await client.AuthenticateAsync(_opt.User, _opt.Password, ct);
				}
				await client.SendAsync(message, ct);
				await client.DisconnectAsync(true, ct);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Не удалось отправить письмо для {To} через {Host}:{Port}", to, _opt.Host, _opt.Port);
				throw new InvalidOperationException("Не удалось отправить письмо. Проверьте настройки SMTP и доступ к серверу.", ex);
			}
		}
	}
}
