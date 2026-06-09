namespace FilmSerileri.Models;

public class BilingualText
{
  public string Tr { get; set; } = string.Empty;
  public string En { get; set; } = string.Empty;

  public string Get(string language) =>
    language.Equals("en", StringComparison.OrdinalIgnoreCase) ? En : Tr;
}
