# Not Defteri

Windows Forms ile geliştirilmiş basit ve kullanışlı bir not defteri uygulaması.

## Geliştirici Bilgileri

- **İsim Soyisim:** Berkay Sabuncu
- **Sınıf:** II/A
- **Bölüm:** Yazılım Mühendisliği, Teknoloji Fakültesi
- **Öğrenci Numarası:** 240542029

## Özellikler

- Yeni dosya oluşturma
- Dosya açma ve kaydetme
- Farklı kaydetme
- Kaydedilmemiş değişiklik uyarısı
- Çoklu encoding desteği (UTF-8, Windows-1254, Windows-1252)
- Komut satırı desteği
- "Birlikte Aç" desteği

## Gereksinimler

- .NET 8.0 SDK
- Windows işletim sistemi

## Kurulum

```bash
git clone https://github.com/brkysbnc/Notebook.git
cd NotDefteri
dotnet build
dotnet run
```

## Kullanım

### Klavye Kısayolları

- `Ctrl+N` - Yeni dosya
- `Ctrl+O` - Dosya aç
- `Ctrl+S` - Kaydet
- `Ctrl+Shift+S` - Farklı kaydet

### Dosya Açma

1. Menüden: Dosya > Aç (Ctrl+O)
2. Birlikte Aç: Dosyaya sağ tıklayın > Birlikte aç > NotDefteri.exe
3. Komut satırı: `NotDefteri.exe "dosya.txt"`

## Teknolojiler

- C# (.NET 8.0)
- Windows Forms
- System.Text.Encoding.CodePages

## Lisans

MIT License
