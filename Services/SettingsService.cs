using System.Text.Json;
using FilmSerileri.Models;

namespace FilmSerileri.Services;

public class SettingsService : ISettingsService
{
  private const string CookieName = "FilmSerileriSettings";
  private readonly IHttpContextAccessor _httpContextAccessor;

  public SettingsService(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public AppSettings GetSettings()
  {
    var context = _httpContextAccessor.HttpContext;
    if (context?.Request.Cookies.TryGetValue(CookieName, out var json) == true && !string.IsNullOrEmpty(json))
    {
      try
      {
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
      }
      catch
      {
        return new AppSettings();
      }
    }

    return new AppSettings();
  }

  public void SaveSettings(AppSettings settings)
  {
    var context = _httpContextAccessor.HttpContext;
    if (context == null) return;

    var json = JsonSerializer.Serialize(settings);
    context.Response.Cookies.Append(CookieName, json, new CookieOptions
    {
      Expires = DateTimeOffset.UtcNow.AddYears(1),
      HttpOnly = false,
      IsEssential = true,
      SameSite = SameSiteMode.Lax
    });
  }
}
