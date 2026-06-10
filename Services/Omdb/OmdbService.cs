using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FilmSerileri.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FilmSerileri.Services.Omdb;

public class OmdbService : IOmdbService
{
  private readonly HttpClient _http;
  private readonly OmdbOptions _options;
  private readonly IDistributedCache _cache;
  private readonly ILogger<OmdbService> _logger;

  public OmdbService(HttpClient http, IOptions<OmdbOptions> options, IDistributedCache cache, ILogger<OmdbService> logger)
  {
    _http = http;
    _options = options.Value;
    _cache = cache;
    _logger = logger;
  }

  public bool IsAvailable => _options.IsConfigured;

  public async Task<double?> GetImdbRatingAsync(string title, int year, CancellationToken ct = default)
  {
    if (!IsAvailable) return null;

    var key = $"omdb:{title}:{year}".ToLowerInvariant();
    try
    {
      var cached = await _cache.GetStringAsync(key, ct);
      if (cached != null && double.TryParse(cached, CultureInfo.InvariantCulture, out var cachedRating))
        return cachedRating;
    }
    catch { /* ignore */ }

    try
    {
      var url = $"{_options.BaseUrl}/?apikey={_options.ApiKey}&t={Uri.EscapeDataString(title)}&y={year}";
      using var response = await _http.GetAsync(url, ct);
      if (!response.IsSuccessStatusCode) return null;

      var json = await response.Content.ReadAsStringAsync(ct);
      var data = JsonSerializer.Deserialize<OmdbResponse>(json);
      if (data?.ImdbRating == null || data.ImdbRating == "N/A") return null;

      if (double.TryParse(data.ImdbRating, NumberStyles.Any, CultureInfo.InvariantCulture, out var rating))
      {
        try
        {
          await _cache.SetStringAsync(key, rating.ToString(CultureInfo.InvariantCulture),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) }, ct);
        }
        catch { /* ignore */ }
        return rating;
      }
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "OMDb request failed for {Title} ({Year})", title, year);
    }

    return null;
  }

  private class OmdbResponse
  {
    [JsonPropertyName("imdbRating")] public string? ImdbRating { get; set; }
  }
}
