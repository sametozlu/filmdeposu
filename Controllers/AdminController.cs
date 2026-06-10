using FilmSerileri.Data;
using FilmSerileri.Entities;
using FilmSerileri.Services;
using FilmSerileri.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmSerileri.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
  private readonly ApplicationDbContext _db;
  private readonly SeriesCatalogService _catalog;
  private readonly IMovieService _movies;

  public AdminController(ApplicationDbContext db, SeriesCatalogService catalog, IMovieService movies)
  {
    _db = db;
    _catalog = catalog;
    _movies = movies;
  }

  public async Task<IActionResult> Index()
  {
    var items = await _db.Series.AsNoTracking()
      .OrderBy(s => s.SortOrder)
      .Select(s => new AdminSeriesListItem
      {
        Id = s.Id,
        Title = s.Title,
        ImdbRating = s.ImdbRating,
        MovieCount = s.Movies.Count,
        SortOrder = s.SortOrder
      })
      .ToListAsync();

    return View(items);
  }

  public IActionResult Create() =>
    View("Edit", new AdminSeriesForm { IsNew = true, ReleaseYearStart = DateTime.Now.Year, ReleaseYearEnd = DateTime.Now.Year });

  public async Task<IActionResult> Edit(string id)
  {
    var entity = await _db.Series.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    if (entity == null) return NotFound();

    return View(new AdminSeriesForm
    {
      IsNew = false,
      Id = entity.Id,
      Title = entity.Title,
      OriginalTitle = entity.OriginalTitle,
      Tagline = entity.Tagline,
      Description = entity.Description,
      Genre = entity.Genre,
      GenreKey = entity.GenreKey,
      ReleaseYearStart = entity.ReleaseYearStart,
      ReleaseYearEnd = entity.ReleaseYearEnd,
      Director = entity.Director,
      Studio = entity.Studio,
      ImdbRating = entity.ImdbRating,
      AccentColor = entity.AccentColor,
      GradientFrom = entity.GradientFrom,
      GradientTo = entity.GradientTo,
      Icon = entity.Icon,
      UniverseId = entity.UniverseId,
      SortOrder = entity.SortOrder,
      Movies = entity.Movies.OrderBy(m => m.Order).Select(m => new AdminMovieRow
      {
        Order = m.Order,
        Title = m.Title,
        Year = m.Year,
        DurationMinutes = m.DurationMinutes,
        Synopsis = m.Synopsis,
        ImdbRating = m.ImdbRating
      }).ToList(),
      Cast = entity.Cast.Select(c => new AdminCastRow
      {
        ActorName = c.ActorName,
        CharacterName = c.CharacterName,
        Role = c.Role
      }).ToList()
    });
  }

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> Save(AdminSeriesForm form)
  {
    form.Movies = form.Movies.Where(m => !string.IsNullOrWhiteSpace(m.Title)).ToList();
    form.Cast = form.Cast.Where(c => !string.IsNullOrWhiteSpace(c.ActorName)).ToList();

    if (!ModelState.IsValid)
      return View("Edit", form);

    var entity = await _db.Series.FirstOrDefaultAsync(s => s.Id == form.Id);

    if (entity == null)
    {
      if (!form.IsNew)
        return NotFound();

      entity = new SeriesEntity { Id = form.Id.Trim().ToLowerInvariant() };
      _db.Series.Add(entity);
    }

    entity.Title = form.Title.Trim();
    entity.OriginalTitle = form.OriginalTitle.Trim();
    entity.Tagline = form.Tagline.Trim();
    entity.Description = form.Description.Trim();
    entity.Genre = form.Genre.Trim();
    entity.GenreKey = string.IsNullOrWhiteSpace(form.GenreKey) ? "fantasy" : form.GenreKey.Trim();
    entity.ReleaseYearStart = form.ReleaseYearStart;
    entity.ReleaseYearEnd = form.ReleaseYearEnd;
    entity.Director = form.Director.Trim();
    entity.Studio = form.Studio.Trim();
    entity.ImdbRating = form.ImdbRating;
    entity.AccentColor = form.AccentColor.Trim();
    entity.GradientFrom = form.GradientFrom.Trim();
    entity.GradientTo = form.GradientTo.Trim();
    entity.Icon = form.Icon.Trim();
    entity.UniverseId = string.IsNullOrWhiteSpace(form.UniverseId) ? null : form.UniverseId.Trim();
    entity.SortOrder = form.SortOrder;

    entity.Movies = form.Movies.Select((m, i) => new SeriesMovieEntity
    {
      Order = m.Order > 0 ? m.Order : i + 1,
      Title = m.Title.Trim(),
      Year = m.Year,
      DurationMinutes = m.DurationMinutes,
      Synopsis = m.Synopsis?.Trim() ?? string.Empty,
      ImdbRating = m.ImdbRating
    }).ToList();

    entity.Cast = form.Cast.Select(c => new SeriesCastEntity
    {
      ActorName = c.ActorName.Trim(),
      CharacterName = c.CharacterName?.Trim() ?? string.Empty,
      Role = string.IsNullOrWhiteSpace(c.Role) ? "Yardımcı Rol" : c.Role.Trim()
    }).ToList();

    await _db.SaveChangesAsync();
    await _catalog.ReloadAsync();
    _movies.InvalidateEnrichedCache();

    TempData["AdminMessage"] = $"\"{entity.Title}\" kaydedildi.";
    return RedirectToAction(nameof(Index));
  }

  public async Task<IActionResult> Reviews()
  {
    var reviews = await _db.Reviews.AsNoTracking()
      .OrderByDescending(r => r.CreatedAt)
      .Take(200)
      .ToListAsync();
    return View(reviews);
  }

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteReview(int id)
  {
    var review = await _db.Reviews.FindAsync(id);
    if (review != null)
    {
      _db.Reviews.Remove(review);
      await _db.SaveChangesAsync();
      TempData["AdminMessage"] = "Yorum silindi.";
    }
    return RedirectToAction(nameof(Reviews));
  }

  [HttpPost, ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(string id)
  {
    var entity = await _db.Series.FirstOrDefaultAsync(s => s.Id == id);
    if (entity != null)
    {
      _db.Series.Remove(entity);
      await _db.SaveChangesAsync();
      await _catalog.ReloadAsync();
      _movies.InvalidateEnrichedCache();
      TempData["AdminMessage"] = $"\"{entity.Title}\" silindi.";
    }

    return RedirectToAction(nameof(Index));
  }
}
