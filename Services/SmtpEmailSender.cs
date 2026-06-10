using FilmSerileri.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FilmSerileri.Services;

/// <summary>MailKit ile gerçek SMTP üzerinden e-posta gönderir (Brevo, Resend, Gmail vb.).</summary>
public class SmtpEmailSender : IAppEmailSender
{
  private readonly EmailOptions _options;
  private readonly ILogger<SmtpEmailSender> _logger;

  public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
  {
    _options = options.Value;
    _logger = logger;
  }

  public async Task SendAsync(string to, string subject, string htmlBody)
  {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
    message.To.Add(MailboxAddress.Parse(to));
    message.Subject = subject;
    message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

    using var client = new SmtpClient();
    try
    {
      await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTlsWhenAvailable);
      if (!string.IsNullOrWhiteSpace(_options.Username))
        await client.AuthenticateAsync(_options.Username, _options.Password);
      await client.SendAsync(message);
    }
    finally
    {
      await client.DisconnectAsync(true);
    }

    _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
  }
}
