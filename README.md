# Film Deposu / Cinema Series

Efsanevi film serilerini tanıtan profesyonel ASP.NET Core MVC uygulaması.

## Özellikler

- **14 film serisi** — Harry Potter, LOTR, Star Wars, Dune, Matrix ve daha fazlası
- **Veritabanı destekli katalog** — seriler EF Core ile DB'de tutulur, ilk açılışta otomatik seed edilir
- **Admin paneli** (`/Admin`) — rol tabanlı içerik yönetimi: seri/film/oyuncu CRUD
- **Yorum + puanlama** — kullanıcılar serilere 1-5 yıldız ve yorum verebilir
- **Kişisel öneri motoru** — kütüphanene göre "Sana Özel Öneriler"
- **İstatistik paneli** (`/Dashboard`) — Chart.js ile tür dağılımı, on yıllar, en iyi seriler
- **SEO** — `sitemap.xml`, `robots.txt`, Open Graph + Twitter Card, JSON-LD structured data
- **Profil + rozet sistemi** (`/Profile`) — izleme istatistikleri ve 8 kazanılabilir rozet
- **Ctrl+K komut paleti** — seri, film ve oyuncularda anlık global arama
- **Film quizi** (`/Quiz`) — katalogdan otomatik üretilen sorular + skor tablosu
- **SignalR canlı bildirimler** — yeni yorumlarda anlık toast bildirimi
- **Şifre sıfırlama akışı** — Identity token tabanlı (dev'de link loga yazılır)
- **Yorum moderasyonu** — admin panelinden tüm yorumları yönetme
- **Prometheus metrikleri** — `/metrics` endpoint'i (prometheus-net)
- **Playwright E2E testleri** — gerçek tarayıcıyla 5 duman testi
- **TMDB + OMDb API** entegrasyonu (fragman, canlı puan, oyuncu biyografisi)
- **REST API v1** + Swagger (`/swagger`) — JWT auth + IP bazlı rate limiting (60 istek/dk)
- **Kullanıcı hesabı** — izleme listesi, izlediklerim, notlar
- **Oyuncu profilleri**, seri karşılaştırma, evren haritaları
- **Film müziği**, kronoloji timeline, benzer seriler, rastgele öneri
- **PWA** desteği, sayfalama, skeleton loading
- **TR/EN** dil, koyu/açık tema, arama & filtreleme
- **Docker Compose** — PostgreSQL + Redis
- **GitHub Actions CI** + xUnit testleri
- **Health check** — `/health`

## Hızlı Başlangıç

```bash
dotnet restore
dotnet run
```

Tarayıcı: `http://localhost:8080`

## API Anahtarları (opsiyonel)

`appsettings.json` veya ortam değişkenleri:

```bash
set TMDB_API_KEY=your_tmdb_key
set OMDB_API_KEY=your_omdb_key
```

Anahtar yoksa uygulama yerel veri ve görsellerle çalışmaya devam eder.

## Docker Compose

```bash
docker compose up --build
```

- Web: `http://localhost:8080`
- PostgreSQL: `5432`
- Redis: `6379`

## Admin Paneli

İlk açılışta otomatik bir admin hesabı oluşturulur (`appsettings.json` → `Admin` bölümü):

- E-posta: `admin@filmdeposu.local`
- Şifre: `Admin123!`

Giriş yaptıktan sonra navbar'daki **Yönetim** linkinden seri ekleyip düzenleyebilirsin.

## REST API (v1)

| Endpoint | Açıklama |
|----------|----------|
| `POST /api/v1/auth/token` | JWT al (`{ "email", "password" }`) |
| `GET /api/v1/series` | Tüm seriler / arama |
| `GET /api/v1/series/{id}` | Seri detayı |
| `GET /api/v1/series/{id}/similar` | Benzer seriler |
| `GET /api/v1/series/random` | Rastgele seri |
| `GET /api/v1/actors` | Oyuncu listesi |
| `GET /api/v1/actors/{slug}` | Oyuncu detayı |
| `GET /api/v1/library/watchlist` | İzleme listesi (JWT gerekli) |
| `POST /api/v1/library/watchlist/{id}` | Listeye ekle/çıkar (JWT gerekli) |
| `GET /api/v1/library/watched` | İzlenenler (JWT gerekli) |
| `GET /health` | Sağlık kontrolü |
| `GET /sitemap.xml` | Site haritası |

Tüm `/api/v1` uçları IP başına dakikada 60 istekle sınırlıdır. Korumalı uçlar için: `Authorization: Bearer <token>`.

## Testler

```bash
# Birim testleri
dotnet test FilmSerileri.Tests

# E2E testleri (uygulama localhost:8080'de çalışıyor olmalı)
dotnet build FilmSerileri.E2E
powershell -ExecutionPolicy Bypass -File FilmSerileri.E2E\bin\Debug\net8.0\playwright.ps1 install chromium  # ilk seferde
dotnet test FilmSerileri.E2E
```

## Poster / Oyuncu Görselleri

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\download-tmdb-posters.ps1
powershell -ExecutionPolicy Bypass -File .\scripts\download-actor-photos.ps1
```

## Teknolojiler

- .NET 8, ASP.NET Core MVC + Web API
- Entity Framework Core (SQLite / PostgreSQL)
- ASP.NET Core Identity
- Serilog, Swagger, Redis cache
- TMDB API, OMDb API
