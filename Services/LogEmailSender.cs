namespace FilmSerileri.Services;

/// <summary>
/// Dev ortamı için e-postaları loga yazar. Production'da SMTP tabanlı
/// bir implementasyonla (ör. MailKit) değiştirilebilir.
/// </summary>
public class LogEmailSender : IAppEmailSender
{
  private readonly ILogger<LogEmailSender> _logger;

  public LogEmailSender(ILogger<LogEmailSender> logger) => _logger = logger;

  public Task SendAsync(string to, string subject, string htmlBody)
  {
    _logger.LogInformation("EMAIL to {To} | {Subject}\n{Body}", to, subject, htmlBody);
    return Task.CompletedTask;
  }
}
