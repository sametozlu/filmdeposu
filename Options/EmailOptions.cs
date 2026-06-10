namespace FilmSerileri.Options;

public class EmailOptions
{
  public const string SectionName = "Email";

  public string Host { get; set; } = string.Empty;
  public int Port { get; set; } = 587;
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string FromAddress { get; set; } = "noreply@filmdeposu.local";
  public string FromName { get; set; } = "Film Deposu";

  public bool IsConfigured => !string.IsNullOrWhiteSpace(Host);
}
