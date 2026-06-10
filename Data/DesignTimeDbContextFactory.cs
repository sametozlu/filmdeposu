using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FilmSerileri.Data;

/// <summary>dotnet ef komutlarının Program.cs'i çalıştırmadan context üretebilmesi için.</summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
  public ApplicationDbContext CreateDbContext(string[] args)
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseSqlite("Data Source=filmdeposu.db")
      .Options;

    return new ApplicationDbContext(options);
  }
}
