# Sinema Serileri / Cinema Series

Efsanevi film serilerini tanıtan modern bir ASP.NET Core MVC web uygulaması.

## Özellikler

- **9 film serisi:** Harry Potter, Yüzüklerin Efendisi, Alacakaranlık, Labirent, Açlık Oyunları, Yıldız Savaşları, Karayip Korsanları, Jurassic Park, Dune
- Film listesi, özet ve IMDb puanları
- Oyuncu kadrosu ve karakter bilgileri
- SVG film posterleri
- Arama ve filtreleme (tür, puan, sıralama)
- Türkçe / İngilizce dil desteği
- Koyu/açık tema, favori seri, kompakt görünüm
- Responsive tasarım

## Yerel Çalıştırma

```bash
dotnet restore
dotnet run
```

Tarayıcıda: `http://localhost:8080`

## Poster Üretimi

Posterler `wwwroot/posters/` altında SVG olarak tutulur. Yeniden üretmek için:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\generate-posters.ps1
```

## Render'da Deploy

1. Projeyi GitHub'a push edin
2. [Render](https://render.com) → **New Web Service**
3. GitHub reposunu bağlayın
4. **Runtime:** Docker
5. Deploy edin

`render.yaml` Blueprint deploy için hazırdır.

## Teknolojiler

- .NET 8
- ASP.NET Core MVC
- Razor Views
- Custom CSS (sinematik tema)
