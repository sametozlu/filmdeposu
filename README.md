# Sinema Serileri

Efsanevi film serilerini tanıtan modern bir ASP.NET Core MVC web uygulaması.

## Özellikler

- Harry Potter, Yüzüklerin Efendisi, Alacakaranlık, Labirent ve Açlık Oyunları
- Her seri için film listesi, özet ve IMDb puanları
- Oyuncu kadrosu ve karakter bilgileri
- Koyu/açık tema, favori seri, kompakt görünüm ayarları
- Responsive tasarım

## Yerel Çalıştırma

```bash
dotnet restore
dotnet run
```

Tarayıcıda: `http://localhost:5000`

## Render'da Deploy

1. Projeyi GitHub'a push edin
2. [Render](https://render.com) hesabı oluşturun
3. **New → Web Service** → GitHub reposunu bağlayın
4. **Runtime:** Docker
5. Deploy edin

`render.yaml` dosyası Blueprint deploy için hazırdır.

## Teknolojiler

- .NET 8
- ASP.NET Core MVC
- Razor Views
- CSS (custom, sinematik tema)
