using System.Text;
using FilmSerileri.Data;
using FilmSerileri.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmSerileri.Controllers;

public class SeoController : Controller
{
  private readonly IMovieService _movies;

  public SeoController(IMovieService movies) => _movies = movies;

  [HttpGet("/sitemap.xml")]
  [ResponseCache(Duration = 3600)]
  public IActionResult Sitemap()
  {
    var baseUrl = $"{Request.Scheme}://{Request.Host}";
    var all = _movies.GetAllSeries("tr");

    var sb = new StringBuilder();
    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
    sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

    void Add(string path, string priority) =>
      sb.AppendLine($"  <url><loc>{baseUrl}{path}</loc><priority>{priority}</priority></url>");

    Add("/", "1.0");
    Add("/Actors", "0.7");
    Add("/Compare", "0.6");
    Add("/Universe", "0.6");
    Add("/Dashboard", "0.5");

    foreach (var series in all)
      Add($"/Series/Detail/{series.Id}", "0.9");

    var actorSlugs = all
      .SelectMany(s => s.Cast)
      .Select(c => ActorSlug.FromName(c.ActorName))
      .Distinct(StringComparer.OrdinalIgnoreCase);

    foreach (var slug in actorSlugs)
      Add($"/Actors/Detail/{slug}", "0.5");

    sb.AppendLine("</urlset>");
    return Content(sb.ToString(), "application/xml", Encoding.UTF8);
  }
}
