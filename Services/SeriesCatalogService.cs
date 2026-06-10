using FilmSerileri.Data;
using FilmSerileri.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmSerileri.Services;

/// <summary>DB destekli seri katalogu; bellekte tutar, admin kaydedince yenilenir.</summary>
public class SeriesCatalogService
{
  private readonly IServiceScopeFactory? _scopeFactory;
  private volatile IReadOnlyList<MovieSeries>? _cache;

  public SeriesCatalogService(IServiceScopeFactory? scopeFactory = null) => _scopeFactory = scopeFactory;

  public IReadOnlyList<MovieSeries> GetAll() => _cache ?? MovieService.BuildSeries();

  public async Task SeedIfEmptyAsync(CancellationToken ct = default)
  {
    if (_scopeFactory == null) return;
    using var scope = _scopeFactory.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (await db.Series.AnyAsync(ct)) return;

    var seed = MovieService.BuildSeries();
    for (var i = 0; i < seed.Count; i++)
      db.Series.Add(SeriesMapper.ToEntity(seed[i], i));

    await db.SaveChangesAsync(ct);
  }

  public async Task ReloadAsync(CancellationToken ct = default)
  {
    if (_scopeFactory == null) return;
    using var scope = _scopeFactory.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var entities = await db.Series.AsNoTracking().OrderBy(s => s.SortOrder).ToListAsync(ct);
    if (entities.Count > 0)
      _cache = entities.Select(SeriesMapper.ToModel).ToList();
  }
}
