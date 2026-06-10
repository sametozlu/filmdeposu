namespace FilmSerileri.Services;

public class TmdbCacheWarmupService : BackgroundService
{
  private readonly IServiceProvider _services;
  private readonly ILogger<TmdbCacheWarmupService> _logger;

  public TmdbCacheWarmupService(IServiceProvider services, ILogger<TmdbCacheWarmupService> logger)
  {
    _services = services;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

    try
    {
      using var scope = _services.CreateScope();
      var movieService = scope.ServiceProvider.GetRequiredService<IMovieService>();
      await movieService.WarmupCacheAsync(stoppingToken);
      _logger.LogInformation("TMDB cache warmup completed");
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "TMDB cache warmup failed");
    }
  }
}
