namespace FilmSerileri.Options;

public class RedisOptions
{
  public const string SectionName = "Redis";
  public bool Enabled { get; set; }
  public string Configuration { get; set; } = "localhost:6379";
}
