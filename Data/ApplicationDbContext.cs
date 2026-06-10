using FilmSerileri.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilmSerileri.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();
  public DbSet<WatchedItem> WatchedItems => Set<WatchedItem>();
  public DbSet<UserNote> UserNotes => Set<UserNote>();
  public DbSet<SeriesEntity> Series => Set<SeriesEntity>();
  public DbSet<Review> Reviews => Set<Review>();
  public DbSet<QuizScore> QuizScores => Set<QuizScore>();

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<SeriesEntity>(e =>
    {
      e.HasKey(x => x.Id);
      e.OwnsMany(x => x.Movies, m =>
      {
        m.ToTable("SeriesMovies");
        m.WithOwner().HasForeignKey("SeriesEntityId");
        m.Property<int>("RowId");
        m.HasKey("RowId");
      });
      e.OwnsMany(x => x.Cast, c =>
      {
        c.ToTable("SeriesCast");
        c.WithOwner().HasForeignKey("SeriesEntityId");
        c.Property<int>("RowId");
        c.HasKey("RowId");
      });
    });

    builder.Entity<Review>(e =>
    {
      e.HasIndex(x => new { x.UserId, x.SeriesId }).IsUnique();
      e.Property(x => x.Text).HasMaxLength(2000);
    });

    builder.Entity<WatchlistItem>(e =>
    {
      e.HasIndex(x => new { x.UserId, x.SeriesId }).IsUnique();
    });

    builder.Entity<WatchedItem>(e =>
    {
      e.HasIndex(x => new { x.UserId, x.SeriesId, x.MovieOrder });
    });

    builder.Entity<UserNote>(e =>
    {
      e.HasIndex(x => new { x.UserId, x.SeriesId }).IsUnique();
    });
  }
}
