using System.Text.Json;
using System.Text.Json.Serialization;
using FilmSerileri.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace FilmSerileri.Services.Tmdb;

public class TmdbService : ITmdbService
{
  private readonly HttpClient _http;
  private readonly TmdbOptions _options;
  private readonly IDistributedCache _cache;
  private readonly ILogger<TmdbService> _logger;
  private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

  public TmdbService(HttpClient http, IOptions<TmdbOptions> options, IDistributedCache cache, ILogger<TmdbService> logger)
  {
    _http = http;
    _options = options.Value;
    _cache = cache;
    _logger = logger;
  }

  public bool IsAvailable => _options.IsConfigured;

  public async Task<TmdbMovieDetails?> GetMovieAsync(int movieId, CancellationToken ct = default)
  {
    if (!IsAvailable) return null;
    return await GetCachedAsync($"tmdb:movie:{movieId}", async () =>
    {
      var url = $"{_options.BaseUrl}/movie/{movieId}?api_key={_options.ApiKey}&language={_options.Language}";
      return await GetJsonAsync<TmdbMovieDetails>(url, ct);
    }, TimeSpan.FromHours(12), ct);
  }

  public async Task<string?> GetTrailerKeyAsync(int movieId, CancellationToken ct = default)
  {
    if (!IsAvailable) return null;
    var videos = await GetCachedAsync($"tmdb:videos:{movieId}", async () =>
    {
      var url = $"{_options.BaseUrl}/movie/{movieId}/videos?api_key={_options.ApiKey}";
      return await GetJsonAsync<TmdbVideosResponse>(url, ct);
    }, TimeSpan.FromDays(1), ct);

    var trailer = videos?.Results
      .Where(v => v.Site == "YouTube" && (v.Type == "Trailer" || v.Type == "Teaser"))
      .OrderByDescending(v => v.Official)
      .ThenBy(v => v.Type == "Trailer" ? 0 : 1)
      .FirstOrDefault();

    return trailer?.Key;
  }

  public async Task<TmdbPersonDetails?> GetPersonAsync(int personId, CancellationToken ct = default)
  {
    if (!IsAvailable) return null;
    return await GetCachedAsync($"tmdb:person:{personId}", async () =>
    {
      var url = $"{_options.BaseUrl}/person/{personId}?api_key={_options.ApiKey}&language={_options.Language}";
      return await GetJsonAsync<TmdbPersonDetails>(url, ct);
    }, TimeSpan.FromDays(7), ct);
  }

  public async Task<IReadOnlyList<int>> GetSimilarMovieIdsAsync(int movieId, CancellationToken ct = default)
  {
    if (!IsAvailable) return Array.Empty<int>();
    var response = await GetCachedAsync($"tmdb:similar:{movieId}", async () =>
    {
      var url = $"{_options.BaseUrl}/movie/{movieId}/similar?api_key={_options.ApiKey}&language={_options.Language}";
      return await GetJsonAsync<TmdbPagedResults>(url, ct);
    }, TimeSpan.FromDays(1), ct);

    return response?.Results?.Select(r => r.Id).Take(6).ToList() ?? new List<int>();
  }

  public async Task<IReadOnlyList<int>> GetCollectionMovieIdsAsync(int collectionId, CancellationToken ct = default)
  {
    if (!IsAvailable) return Array.Empty<int>();
    var response = await GetCachedAsync($"tmdb:collection:{collectionId}", async () =>
    {
      var url = $"{_options.BaseUrl}/collection/{collectionId}?api_key={_options.ApiKey}&language={_options.Language}";
      return await GetJsonAsync<TmdbCollectionResponse>(url, ct);
    }, TimeSpan.FromDays(1), ct);

    return response?.Parts?.Select(p => p.Id).ToList() ?? new List<int>();
  }

  private async Task<T?> GetCachedAsync<T>(string key, Func<Task<T?>> factory, TimeSpan ttl, CancellationToken ct) where T : class
  {
    try
    {
      var cached = await _cache.GetStringAsync(key, ct);
      if (cached != null)
        return JsonSerializer.Deserialize<T>(cached, JsonOpts);
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Cache read failed for {Key}", key);
    }

    var value = await factory();
    if (value == null) return null;

    try
    {
      await _cache.SetStringAsync(key, JsonSerializer.Serialize(value, JsonOpts),
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Cache write failed for {Key}", key);
    }

    return value;
  }

  private async Task<T?> GetJsonAsync<T>(string url, CancellationToken ct) where T : class
  {
    try
    {
      using var response = await _http.GetAsync(url, ct);
      if (!response.IsSuccessStatusCode)
      {
        _logger.LogWarning("TMDB request failed {Status} for {Url}", response.StatusCode, url);
        return null;
      }
      await using var stream = await response.Content.ReadAsStreamAsync(ct);
      return await JsonSerializer.DeserializeAsync<T>(stream, JsonOpts, ct);
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "TMDB request error for {Url}", url);
      return null;
    }
  }

  private class TmdbPagedResults
  {
    [JsonPropertyName("results")] public List<TmdbIdResult>? Results { get; set; }
  }

  private class TmdbIdResult
  {
    [JsonPropertyName("id")] public int Id { get; set; }
  }
}
