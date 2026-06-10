using System.Text;
using System.Threading.RateLimiting;
using FilmSerileri.Data;
using FilmSerileri.Entities;
using FilmSerileri.Hubs;
using FilmSerileri.Options;
using FilmSerileri.Services;
using FilmSerileri.Services.Omdb;
using FilmSerileri.Services.Tmdb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

Log.Logger = new LoggerConfiguration()
  .WriteTo.Console()
  .CreateBootstrapLogger();

try
{
  var builder = WebApplication.CreateBuilder(args);

  builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

  builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

  builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection(TmdbOptions.SectionName));
  builder.Services.Configure<OmdbOptions>(builder.Configuration.GetSection(OmdbOptions.SectionName));
  builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));
  builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));

  var tmdbKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");
  if (!string.IsNullOrWhiteSpace(tmdbKey))
    builder.Configuration["Tmdb:ApiKey"] = tmdbKey;

  var omdbKey = Environment.GetEnvironmentVariable("OMDB_API_KEY");
  if (!string.IsNullOrWhiteSpace(omdbKey))
    builder.Configuration["Omdb:ApiKey"] = omdbKey;

  var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=filmdeposu.db";

  if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
      connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
  {
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])};SSL Mode=Prefer;Trust Server Certificate=true";
  }

  builder.Services.AddDbContext<ApplicationDbContext>(options =>
  {
    if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
      options.UseNpgsql(connectionString);
    else
      options.UseSqlite(connectionString);
  });

  builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
  {
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
  })
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

  builder.Services.ConfigureApplicationCookie(options =>
  {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
  });

  var jwtKey = builder.Configuration["Jwt:Key"] ?? "FilmDeposu-dev-secret-key-degistir-32char!";
  builder.Configuration["Jwt:Key"] = jwtKey;
  builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "FilmDeposu",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "FilmDeposuApi",
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
      };
    });

  builder.Services.AddRateLimiter(options =>
  {
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("api", httpContext =>
      RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
          PermitLimit = 60,
          Window = TimeSpan.FromMinutes(1),
          QueueLimit = 0
        }));
  });

  var redisOptions = builder.Configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>() ?? new RedisOptions();
  if (redisOptions.Enabled)
    builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redisOptions.Configuration);
  else
    builder.Services.AddDistributedMemoryCache();

  builder.Services.AddMemoryCache();
  builder.Services.AddHttpClient<ITmdbService, TmdbService>();
  builder.Services.AddHttpClient<IOmdbService, OmdbService>();

  builder.Services.AddSingleton<MovieEnrichmentService>();
  builder.Services.AddSingleton(sp => new SeriesCatalogService(sp.GetRequiredService<IServiceScopeFactory>()));
  builder.Services.AddSingleton<IMovieService, MovieService>();
  builder.Services.AddScoped<ISettingsService, SettingsService>();
  builder.Services.AddScoped<ILocalizationService, LocalizationService>();
  builder.Services.AddScoped<IUserLibraryService, UserLibraryService>();
  builder.Services.AddScoped<IReviewService, ReviewService>();
  builder.Services.AddScoped<IRecommendationService, RecommendationService>();
  var emailOptions = builder.Configuration.GetSection(EmailOptions.SectionName).Get<EmailOptions>() ?? new EmailOptions();
  if (emailOptions.IsConfigured)
    builder.Services.AddSingleton<IAppEmailSender, SmtpEmailSender>();
  else
    builder.Services.AddSingleton<IAppEmailSender, LogEmailSender>();
  builder.Services.AddSignalR();
  builder.Services.AddHostedService<TmdbCacheWarmupService>();

  builder.Services.AddHttpContextAccessor();
  builder.Services.AddControllersWithViews();
  builder.Services.AddControllers();
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Film Deposu API", Version = "v1" }));

  builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

  var app = builder.Build();

  using (var scope = app.Services.CreateScope())
  {
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // SQLite: migration tabanlı şema (veri kaybı olmadan güncellenir).
    // PostgreSQL: migration'lar SQLite için üretildiğinden EnsureCreated kullanılır.
    if (db.Database.IsSqlite())
      await db.Database.MigrateAsync();
    else
      await db.Database.EnsureCreatedAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Admin"))
      await roleManager.CreateAsync(new IdentityRole("Admin"));

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = builder.Configuration["Admin:Email"] ?? "admin@filmdeposu.local";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
      admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, DisplayName = "Admin", EmailConfirmed = true };
      await userManager.CreateAsync(admin, builder.Configuration["Admin:Password"] ?? "Admin123!");
    }
    if (!await userManager.IsInRoleAsync(admin, "Admin"))
      await userManager.AddToRoleAsync(admin, "Admin");
  }

  var catalog = app.Services.GetRequiredService<SeriesCatalogService>();
  await catalog.SeedIfEmptyAsync();
  await catalog.ReloadAsync();

  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Film Deposu API v1"));
  }
  else
  {
    app.UseExceptionHandler("/Home/Error");
  }

  app.UseSerilogRequestLogging();
  app.UseStaticFiles();
  app.UseRouting();
  app.UseRateLimiter();
  app.UseHttpMetrics();
  app.UseAuthentication();
  app.UseAuthorization();

  app.MapHealthChecks("/health");
  app.MapMetrics();
  app.MapHub<NotificationHub>("/hubs/notifications");
  app.MapControllers();
  app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

  await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}
