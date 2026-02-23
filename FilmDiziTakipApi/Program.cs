using FilmDiziTakipApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using FilmDiziTakipApi.Models;
using Microsoft.AspNetCore.Mvc;
using FilmDiziTakipApi.Models.Dtos;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


// ✅ Veritabanı bağlantısı ekleniyor (DOĞRU YERDE)
builder.Services.AddDbContext<UygulamaDB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// ✅ CORS tanımı
builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        
        policy.WithOrigins("https://localhost:7248")  
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); 
    });
});

var app = builder.Build();



app.UseCors("AllowSpecificOrigin");



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region yonetici

app.MapPost("/yonetici/giris", async (UygulamaDB db, [FromBody] YoneticiEkleModel dto) =>
{
    var yonetici = await db.Yonetici
        .FirstOrDefaultAsync(x => x.YoneticiAdi == dto.KullaniciAdi && x.Sifre == dto.Sifre);

    if (yonetici == null)
        return Results.Unauthorized();

    return Results.Ok(yonetici);
}).WithTags("Yönetici");


app.MapPost("/yonetici/ekle", async (UygulamaDB db, [FromBody] YoneticiEkleModel yeniYonetici) =>
{
    // Yeni yöneticiyi ekle
    var yonetici = new Yonetici
    {
        YoneticiAdi = yeniYonetici.KullaniciAdi,
        Sifre = yeniYonetici.Sifre,
        OlusturulmaTarihi = DateTime.Now // OlusturulmaTarihi'ni ekle
    };

    db.Yonetici.Add(yonetici);
    await db.SaveChangesAsync();

    // Başarılı bir şekilde eklenen yönetici döndürülür
    return Results.Created($"/yonetici/{yonetici.Id}", yonetici);
}).WithTags("Yönetici");


app.MapGet("/yonetici/bul", async (UygulamaDB db, string? yoneticiAdi) =>
{
    var yoneticiler = string.IsNullOrEmpty(yoneticiAdi)
        ? await db.Yonetici.ToListAsync()
        : await db.Yonetici
            .Where(x => x.YoneticiAdi.Contains(yoneticiAdi))
            .ToListAsync();

    return yoneticiler.Any() ? Results.Ok(yoneticiler) : Results.NoContent();
}).WithTags("Yönetici");

app.MapGet("/yonetici/listele", async (UygulamaDB db) =>
{
    var yoneticiler = await db.Yonetici.ToListAsync();
    return yoneticiler.Any() ? Results.Ok(yoneticiler) : Results.NoContent();
}).WithTags("Yönetici");

app.MapDelete("/yonetici/sil/{id}", async (UygulamaDB db, int id) =>
{
    var yonetici = await db.Yonetici.FindAsync(id);

    if (yonetici == null)
        return Results.NotFound(); // Yönetici bulunamazsa 404 döner

    db.Yonetici.Remove(yonetici);
    await db.SaveChangesAsync();

    return Results.Ok($"Yönetici ID {id} başarıyla silindi."); // Başarılı olursa 200 döner
}).WithTags("Yönetici");


#endregion yonetici

#region kullanici

app.MapPost("/kullanici/giris", async (UygulamaDB db, [FromBody] KullaniciEkleModel dto) =>
{
    var kullanici = await db.Kullanici
        .FirstOrDefaultAsync(x => x.KullaniciAdi == dto.KullaniciAdi && x.Sifre == dto.Sifre);

    if (kullanici == null)
        return Results.Unauthorized();

    return Results.Ok(kullanici);
}).WithTags("Kullanıcı");

app.MapPost("/kullanicilar/sifredegistir", async (SifreDegistirDto dto, UygulamaDB db) =>
{
    var kullanici = await db.Kullanici.FindAsync(dto.KullaniciId);

    if (kullanici is null)
        return Results.NotFound("Kullanıcı bulunamadı.");

    if (kullanici.Sifre != dto.EskiSifre)
        return Results.BadRequest("Eski şifre yanlış.");

    if (dto.YeniSifre.Length < 6)
        return Results.BadRequest("Yeni şifre en az 6 karakter olmalı.");

    kullanici.Sifre = dto.YeniSifre;
    await db.SaveChangesAsync();

    return Results.Ok("Şifre başarıyla güncellendi.");
}).WithTags("Kullanıcı");

app.MapPost("/kullanici/ekle", async (UygulamaDB db, [FromBody] KullaniciEkleModel yeniKullanici) =>
{
    // Yeni yöneticiyi ekle
    var kullanici = new Kullanici
    {
        KullaniciAdi = yeniKullanici.KullaniciAdi,
        Sifre = yeniKullanici.Sifre,
        OlusturulmaTarihi = DateTime.Now // OlusturulmaTarihi'ni ekle
    };

    db.Kullanici.Add(kullanici);
    await db.SaveChangesAsync();

    // Başarılı bir şekilde eklenen yönetici döndürülür
    return Results.Created($"/kullanici/{kullanici.Id}", kullanici);
}).WithTags("Kullanıcı");

app.MapPut("/kullanici/guncelle/{id}", async (int id, UygulamaDB db, [FromBody] Kullanici guncelKullanici) =>
{
    var mevcutKullanici = await db.Kullanici.FindAsync(id);

    if (mevcutKullanici is null)
        return Results.NotFound("Kullanıcı bulunamadı.");

    mevcutKullanici.KullaniciAdi = guncelKullanici.KullaniciAdi;
    mevcutKullanici.Sifre = guncelKullanici.Sifre;
    mevcutKullanici.OlusturulmaTarihi = guncelKullanici.OlusturulmaTarihi;

    await db.SaveChangesAsync();

    return Results.Ok(mevcutKullanici);
}).WithTags("Kullanıcı");


app.MapGet("/kullanici/bul", async (UygulamaDB db, string? kullaniciAdi) =>
{
    var kullanicilar = string.IsNullOrEmpty(kullaniciAdi)
        ? await db.Kullanici.ToListAsync()
        : await db.Kullanici
            .Where(x => x.KullaniciAdi == kullaniciAdi)
            .ToListAsync(); 

    return kullanicilar.Any() ? Results.Ok(kullanicilar) : Results.NoContent();
}).WithTags("Kullanıcı");


app.MapGet("/kullanici/listele", async (UygulamaDB db) =>
{
    var kullanicilar = await db.Kullanici.ToListAsync();
    return kullanicilar.Any() ? Results.Ok(kullanicilar) : Results.NoContent();
}).WithTags("Kullanıcı");

app.MapDelete("/kullanici/sil/{id}", async (UygulamaDB db, int id) =>
{
    var kullanici = await db.Kullanici.FindAsync(id);

    if (kullanici == null)
        return Results.NotFound();

    db.Kullanici.Remove(kullanici);
    await db.SaveChangesAsync();

    return Results.Ok($"Kullanıcı ID {id} başarıyla silindi."); // Başarılı olursa 200 döner
}).WithTags("Kullanıcı");

#endregion kullanici

#region film

// Film eklemek için endpoint
app.MapGet("/film/listele", async (UygulamaDB db) =>
{
    var filmler = await db.Film.ToListAsync();
    return Results.Ok(filmler);
}).WithTags("Film");

app.MapGet("/film/bul/{id}", async (UygulamaDB db, int id) =>
{
    var film = await db.Film.FindAsync(id);
    return film is not null ? Results.Ok(film) : Results.NotFound();
}).WithTags("Film");

app.MapGet("/film/kategoriye_gore_listele/{kategoriId}", async (UygulamaDB db, int kategoriId) =>
{
    var filmler = await db.Film
        .Where(f => f.KategoriId == kategoriId)
        .ToListAsync();
    return Results.Ok(filmler);
}).WithTags("Film");

app.MapGet("/api/filmler/ara", async (string ad, UygulamaDB db) =>
{
    if (string.IsNullOrWhiteSpace(ad))
        return Results.BadRequest("Arama kelimesi boş olamaz.");

    // Türkçe karakterleri normalize eden yardımcı fonksiyon
    string Normalize(string text) =>
        text.ToLower()
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u");

    string normalizedQuery = Normalize(ad);

    var filmler = await db.Film.ToListAsync();

    // Basit Levenshtein implementasyonu
    int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        int[,] d = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) d[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[s.Length, t.Length];
    }

    var eslesenFilmler = filmler
        .Where(f =>
        {
            string normalizedAd = Normalize(f.Ad);
            return normalizedAd.Contains(normalizedQuery) ||
                   LevenshteinDistance(normalizedAd, normalizedQuery) <= 2;
        })
        .ToList();

    return Results.Ok(eslesenFilmler);
}).WithTags("Film");

app.MapPost("/film/ekle", async (UygulamaDB db, [FromBody] FilmEkleModel filmDTO) =>
{
    // KategoriID'yi kullanarak kategoriyi veritabanından al
    var kategori = await db.Kategori.FindAsync(filmDTO.KategoriId);

    if (kategori == null)
        return Results.BadRequest("Geçersiz kategori ID'si.");

    // Yeni film nesnesini oluştur
    var film = new Film
    {
        Ad = filmDTO.Ad,
        Aciklama = filmDTO.Aciklama,
        YayınTarihi = filmDTO.YayınTarihi,
        Yonetmen = filmDTO.Yonetmen,
        CikisTarihi = filmDTO.CikisTarihi,
        KategoriId = filmDTO.KategoriId,
        ImageUrl = filmDTO.ImageUrl,
        FilmUrl = filmDTO.FilmUrl,
       
    };

    // Veritabanına ekle
    db.Film.Add(film);
    await db.SaveChangesAsync();

    // Başarılı bir şekilde ekledikten sonra film bilgilerini döndür
    return Results.Created($"/film/{film.Id}", film);
}).WithTags("Film");

app.MapPut("/film/guncelle/{id}", async (UygulamaDB db, int id, Film film) =>
{
    if (id != film.Id)
        return Results.BadRequest();

    var mevcutFilm = await db.Film.FindAsync(id);
    if (mevcutFilm is null)
        return Results.NotFound();

    mevcutFilm.Ad = film.Ad;
    mevcutFilm.Aciklama = film.Aciklama;
    mevcutFilm.YayınTarihi = film.YayınTarihi;
    mevcutFilm.Yonetmen = film.Yonetmen;
    mevcutFilm.CikisTarihi = film.CikisTarihi;
    mevcutFilm.KategoriId = film.KategoriId;
    mevcutFilm.ImageUrl = film.ImageUrl;
    mevcutFilm.FilmUrl = film.FilmUrl;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Film");

app.MapDelete("/film/sil", async (UygulamaDB db, int id) =>
{
    var film = await db.Film.FindAsync(id);
    if (film == null)
        return Results.NotFound("Film bulunamadı.");

    db.Film.Remove(film);
    await db.SaveChangesAsync();

    return Results.Ok();
}).WithTags("Film");


#endregion film

#region dizi

// Film eklemek için endpoint
app.MapPost("/dizi/ekle", async (UygulamaDB db, DiziEkleModel diziDTO) =>
{
    // Kategori kontrolü
    var kategori = await db.Kategori.FindAsync(diziDTO.KategoriId);
    if (kategori == null)
        return Results.BadRequest("Geçersiz kategori ID'si.");



    // Yeni Dizi nesnesi oluştur
    var dizi = new Dizi
    {
        Ad = diziDTO.Ad,
        Aciklama = diziDTO.Aciklama,
        YayınTarihi = diziDTO.YayınTarihi,
        YuklenmeTarihi = DateTime.Now,
        Yonetmen = diziDTO.Yonetmen,
        KategoriId = diziDTO.KategoriId,
       

        Kategori = kategori
    };

    // Veritabanına ekle
    db.Dizi.Add(dizi);
    await db.SaveChangesAsync();

    return Results.Created($"/dizi/{dizi.Id}", dizi);
}).WithTags("Dizi");

// Tüm dizileri listelemek için endpoint
app.MapGet("/dizi/listele", async (UygulamaDB db) =>
{
    var diziler = await db.Dizi.Include(f => f.Kategori).ToListAsync();
    return Results.Ok(diziler);
}).WithTags("Dizi");

app.MapGet("/dizi/kategoriye_gore_listele/{kategoriId}", async (UygulamaDB db, int kategoriId) =>
{
    var diziler = await db.Dizi
        .Where(f => f.KategoriId == kategoriId)
        .ToListAsync();
    return Results.Ok(diziler);
}).WithTags("Dizi");

app.MapGet("/api/diziler/ara", async (string ad, UygulamaDB db) =>
{
    if (string.IsNullOrWhiteSpace(ad))
        return Results.BadRequest("Arama kelimesi boş olamaz.");

    // Türkçe karakterleri normalize eden yardımcı fonksiyon
    string Normalize(string text) =>
        text.ToLower()
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u");

    string normalizedQuery = Normalize(ad);

    var diziler = await db.Dizi.ToListAsync();

    // Basit Levenshtein implementasyonu
    int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        int[,] d = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) d[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[s.Length, t.Length];
    }

    var eslesenDiziler = diziler
        .Where(f =>
        {
            string normalizedAd = Normalize(f.Ad);
            return normalizedAd.Contains(normalizedQuery) ||
                   LevenshteinDistance(normalizedAd, normalizedQuery) <= 2;
        })
        .ToList();

    return Results.Ok(eslesenDiziler);
}).WithTags("Dizi");

// ID'ye göre tek bir diziyi döndürmek için endpoint
app.MapGet("/dizi/bul/{id}", async (UygulamaDB db, int id) =>
{
    var dizi = await db.Dizi.Include(f => f.Kategori).FirstOrDefaultAsync(f => f.Id == id);

    if (dizi == null)
        return Results.NotFound();

    return Results.Ok(dizi);
}).WithTags("Dizi");

// Dizi güncellemek için endpoint
app.MapPut("/dizi/guncelle/{id}", async (UygulamaDB db, int id, DiziEkleModel dizi) =>
{
    var mevcutDizi = await db.Dizi.FindAsync(id);

    if (mevcutDizi == null)
        return Results.NotFound();

    mevcutDizi.Ad = dizi.Ad;
    mevcutDizi.Aciklama = dizi.Aciklama;
    mevcutDizi.YayınTarihi = dizi.YayınTarihi;
    mevcutDizi.Yonetmen = dizi.Yonetmen;
    mevcutDizi.KategoriId = dizi.KategoriId;
    
    mevcutDizi.ImageUrl = dizi.ImageUrl;
    mevcutDizi.Puan = dizi.Puan;

    try
    {
        await db.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }

    return Results.Ok(mevcutDizi);
}).WithTags("Dizi");


// Dizi silmek için endpoint
app.MapDelete("/dizi/{id}", async (UygulamaDB db, int id) =>
{
    var dizi = await db.Dizi.FindAsync(id);

    if (dizi == null)
        return Results.NotFound();

    db.Dizi.Remove(dizi);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).WithTags("Dizi");

app.MapDelete("/dizi/tumunu-sil", async (UygulamaDB db) =>
{
    var diziler = await db.Dizi.ToListAsync();
    db.Dizi.RemoveRange(diziler);
    await db.SaveChangesAsync();

    return Results.Ok("Tüm diziler silindi.");
}).WithTags("Dizi");

#endregion dizi

#region dizi_Sezon_Bolum

app.MapGet("/api/diziler/{diziId}/sezonlar", async (int diziId, UygulamaDB db) =>
{
    var sezonlar = await db.DiziSezon
        .Where(s => s.DiziId == diziId)
        .Select(s => new
        {
            s.Id,
            s.SezonNo

        })
        .ToListAsync();

    return Results.Ok(sezonlar);
}).WithTags("Dizi Sezon Bölüm");

app.MapGet("/api/sezonlar/{sezonId}/bolumler", async (int sezonId, UygulamaDB db) =>
{
    var bolumler = await db.DiziBolum
        .Where(b => b.SezonId == sezonId)
        .Select(b => new
        {
            b.Id,
            b.BolumNo,
            b.Ad,
            b.ThumbnailUrl,
           
        })
        .ToListAsync();

    return Results.Ok(bolumler);
}).WithTags("Dizi Sezon Bölüm");

app.MapGet("/api/bolumler/{id}", async (int id, UygulamaDB db) =>
{
    var bolum = await db.DiziBolum.FindAsync(id);

    return bolum is not null ? Results.Ok(bolum) : Results.NotFound();
}).WithTags("Dizi Sezon Bölüm");

app.MapGet("/api/diziler/{diziId}/sezonlar_ve_bolumler", async (int diziId, UygulamaDB db) =>
{
    var sezonlar = await db.DiziSezon
        .Where(s => s.DiziId == diziId)
        .Include(s => s.Bolumler)
        .Select(s => new
        {
            s.Id,
            s.SezonNo,
            Bolumler = s.Bolumler.Select(b => new
            {
                b.Id,
                b.BolumNo,
                b.Ad,
                b.ThumbnailUrl
            }).ToList()
        })
        .ToListAsync();

    return Results.Ok(sezonlar);
}).WithTags("Dizi Sezon Bölüm");

app.MapGet("/bolumler/{id}/sezon", async (int id, UygulamaDB db) =>
{
    var bolum = await db.DiziBolum
        .Include(b => b.Sezon) // 🔁 İlişkili sezona erişim
        .FirstOrDefaultAsync(b => b.Id == id);

    if (bolum == null || bolum.Sezon == null)
        return Results.NotFound("Bölüm ya da sezon bulunamadı.");

    return Results.Ok(new BolumSezonDto { SezonNo = bolum.Sezon.SezonNo });
}).WithTags("Dizi Sezon Bölüm");

app.MapGet("/bolumler/{bolumId}/sonraki", async (int bolumId, UygulamaDB db) =>
{
    var mevcut = await db.DiziBolum.FindAsync(bolumId);
    if (mevcut == null) return Results.NotFound();

    // Aynı sezonda sonraki bölüm
    var sonraki = await db.DiziBolum.FirstOrDefaultAsync(b =>
        b.SezonId == mevcut.SezonId && b.BolumNo == mevcut.BolumNo + 1);

    if (sonraki != null)
        return Results.Ok(sonraki);

    // Sezon bilgisini çek
    var mevcutSezon = await db.DiziSezon.FindAsync(mevcut.SezonId);
    if (mevcutSezon == null) return Results.NotFound();

    // Aynı dizide bir sonraki sezonun ilk bölümü
    var sonrakiSezon = await db.DiziSezon
        .Where(s => s.DiziId == mevcutSezon.DiziId && s.SezonNo == mevcutSezon.SezonNo + 1)
        .FirstOrDefaultAsync();

    if (sonrakiSezon == null) return Results.NotFound();

    var ilkBolum = await db.DiziBolum
        .Where(b => b.SezonId == sonrakiSezon.Id)
        .OrderBy(b => b.BolumNo)
        .FirstOrDefaultAsync();

    return ilkBolum != null ? Results.Ok(ilkBolum) : Results.NotFound();
}).WithTags("Dizi Sezon Bölüm");

app.MapPost("/api/sezonlar/ekle", async (DiziSezon sezon, UygulamaDB db) =>
{
    if (sezon == null || sezon.DiziId <= 0 || sezon.SezonNo <= 0)
        return Results.BadRequest("Geçersiz sezon bilgisi.");

    db.DiziSezon.Add(sezon);
    await db.SaveChangesAsync();

    return Results.Created($"/api/sezonlar/{sezon.Id}", sezon);
}).WithTags("Dizi Sezon Bölüm");


app.MapPost("/api/bolumler", async (DiziBolum bolum, UygulamaDB db) =>
{
    if (bolum == null || bolum.SezonId <= 0 || string.IsNullOrWhiteSpace(bolum.Ad))
        return Results.BadRequest("Geçersiz bölüm bilgisi.");

    // Otomatik eklenme tarihi için CreatedDate gibi bir alan varsa burada atanabilir
    // Eğer modelde yoksa ve API’da gerekli ise ekleyebiliriz.

    db.DiziBolum.Add(bolum);
    await db.SaveChangesAsync();

    return Results.Created($"/api/bolumler/{bolum.Id}", bolum);
}).WithTags("Dizi Sezon Bölüm");

app.MapDelete("/api/sezonlar/{id:int}", async (int id, UygulamaDB db) =>
{
    var sezon = await db.DiziSezon.FindAsync(id);
    if (sezon is null)
        return Results.NotFound("Sezon bulunamadı.");

    // Bölümler varsa önce sil (Cascade yoksa)
    var bolumler = db.DiziBolum.Where(b => b.SezonId == id);
    db.DiziBolum.RemoveRange(bolumler);

    db.DiziSezon.Remove(sezon);
    await db.SaveChangesAsync();

    return Results.Ok("Sezon silindi.");
});


#endregion dizi_Sezon_Bolum

#region film_ve_dizi_izleme
app.MapPost("/izleme-durumu/ekle", async (UygulamaDB db, IzlemeDurumuFilm istek) =>
{
    var mevcut = await db.IzlemeDurumuFilm
        .FirstOrDefaultAsync(x => x.KullaniciId == istek.KullaniciId && x.FilmId == istek.FilmId);

    if (mevcut != null)
    {
        mevcut.KalanSure = istek.KalanSure;
        db.IzlemeDurumuFilm.Update(mevcut);
    }
    else
    {
       
        await db.IzlemeDurumuFilm.AddAsync(istek);
    }
    istek.YayinlanmaTarihi = DateTime.Now; 
    await db.SaveChangesAsync();
    return Results.Ok(istek);
}).WithTags("izleme durumu");



app.MapGet("/izleme-durumu/filmler", async (int kullaniciId, UygulamaDB db) =>
{
    var izlenenFilmler = await db.IzlemeDurumuFilm
        .Where(x => x.KullaniciId == kullaniciId)
        .Include(x => x.Film) // Eğer Film navigasyon özelliği varsa
        .Select(x => x.Film) // Sadece filmi dön
        .ToListAsync();

    return Results.Ok(izlenenFilmler);
}).WithTags("izleme durumu");


app.MapGet("/izleme-durumu/kalan-sure", async (UygulamaDB db, int kullaniciId, int filmId) =>
{
    var kayit = await db.IzlemeDurumuFilm
        .FirstOrDefaultAsync(x => x.KullaniciId == kullaniciId && x.FilmId == filmId);

    if (kayit == null)
        return Results.Ok(0.0); // Kalan süre yoksa 0 döndür

    return Results.Ok(kayit.KalanSure);
}).WithTags("izleme durumu");

app.MapDelete("/izleme-durumu/sil", async (UygulamaDB db, int kullaniciId, int filmId) =>
{
    var kayit = await db.IzlemeDurumuFilm
        .FirstOrDefaultAsync(x => x.KullaniciId == kullaniciId && x.FilmId == filmId);

    if (kayit == null)
        return Results.NotFound("Kayıt bulunamadı.");

    db.IzlemeDurumuFilm.Remove(kayit);
    await db.SaveChangesAsync();

    return Results.Ok("Film geçmişten silindi.");
}).WithTags("izleme durumu");

app.MapDelete("/izleme-durumu/tumunu-sil", async (UygulamaDB db, int kullaniciId) =>
{
    var kayitlar = await db.IzlemeDurumuFilm
        .Where(x => x.KullaniciId == kullaniciId)
        .ToListAsync();

    if (!kayitlar.Any())
        return Results.NotFound("Silinecek kayıt bulunamadı.");

    db.IzlemeDurumuFilm.RemoveRange(kayitlar);
    await db.SaveChangesAsync();

    return Results.Ok("Tüm izleme geçmişi silindi.");
}).WithTags("izleme durumu");

app.MapPost("/izleme-durumu_dizi/ekle", async (IzlemeDurumuDiziDto dto, UygulamaDB db) =>
{
    var mevcut = await db.IzlemeDurumuDizi
        .FirstOrDefaultAsync(x => x.DiziBolumId == dto.DiziBolumId && x.KullaniciId == dto.KullaniciId);

    if (mevcut != null)
    {
        // Güncelleme
        mevcut.IzlenmeSuresi = dto.IzlenmeSuresi;
        mevcut.IzlenmeTarihi = DateTime.Now;
    }
    else
    {
        // Yeni ekleme
        var yeni = new IzlemeDurumuDizi
        {
            DiziBolumId = dto.DiziBolumId,
            KullaniciId = dto.KullaniciId,
            IzlenmeSuresi = dto.IzlenmeSuresi,
            IzlenmeTarihi = DateTime.Now
        };
        db.IzlemeDurumuDizi.Add(yeni);
    }

    await db.SaveChangesAsync();
    return Results.Ok();
}).WithTags("izleme durumu");

app.MapGet("/izleme-durumu_dizi/sure", async (int kullaniciId, int bolumId, UygulamaDB db) =>
{
    var izleme = await db.IzlemeDurumuDizi
        .FirstOrDefaultAsync(x => x.KullaniciId == kullaniciId && x.DiziBolumId == bolumId);

    return Results.Ok(izleme?.IzlenmeSuresi ?? 0);
}).WithTags("izleme durumu");

app.MapGet("/izleme-durumu_dizi/liste", async (int kullaniciId, UygulamaDB db) =>
{
    var gecmis = await db.IzlemeDurumuDizi
        .Where(x => x.KullaniciId == kullaniciId)
        .Include(x => x.DiziBolum)
            .ThenInclude(b => b.Sezon)
                .ThenInclude(s => s.Dizi)
        .Select(x => new IzlemeDetayModel
        {
            BolumId= x.DiziBolumId,
            DiziId = x.DiziBolum.Sezon.Dizi.Id,
            DiziAdi = x.DiziBolum.Sezon.Dizi.Ad,
            SezonNo = x.DiziBolum.Sezon.SezonNo,
            BolumNo = x.DiziBolum.BolumNo,
            KalanSure = x.IzlenmeSuresi,
            AfisUrl = x.DiziBolum.Sezon.Dizi.ImageUrl ?? string.Empty
        })
        .ToListAsync();

    return Results.Ok(gecmis);
}).WithTags("izleme durumu");

app.MapDelete("/izleme-durumu-dizi/sil", async (int kullaniciId, int diziBolumId, UygulamaDB db) =>
{
    var kayit = await db.IzlemeDurumuDizi
        .FirstOrDefaultAsync(x => x.KullaniciId == kullaniciId && x.DiziBolumId == diziBolumId);

    if (kayit == null)
        return Results.NotFound("İzleme kaydı bulunamadı.");

    db.IzlemeDurumuDizi.Remove(kayit);
    await db.SaveChangesAsync();

    return Results.Ok("Silindi.");
}).WithTags("izleme durumu");

app.MapDelete("/izleme-durumu-dizi/tumunu-sil", async (int kullaniciId, UygulamaDB db) =>
{
    var kayitlar = db.IzlemeDurumuDizi.Where(x => x.KullaniciId == kullaniciId);

    if (!kayitlar.Any())
        return Results.NotFound("Kullanıcıya ait izleme geçmişi bulunamadı.");

    db.IzlemeDurumuDizi.RemoveRange(kayitlar);
    await db.SaveChangesAsync();

    return Results.Ok("Tüm dizi izleme geçmişi başarıyla silindi.");
}).WithTags("izleme durumu");

app.MapGet("/dizi/{diziId}/bolumler", async (int diziId, UygulamaDB db) =>
{
    var bolumler = await db.DiziBolum
        .Where(b => b.Sezon.DiziId == diziId)
        .OrderBy(b => b.Sezon.SezonNo)
        .ThenBy(b => b.BolumNo)
        .ToListAsync();

    return Results.Ok(bolumler);
}).WithTags("izleme durumu");




#endregion

#region kategori

app.MapPost("/kategori/ekle", async (UygulamaDB db, [FromBody] KategoriEkleModel yeniKategori) =>
{
    // Yeni yöneticiyi ekle
    var kategori = new Kategori
    {
        Ad = yeniKategori.Ad,
    };

    db.Kategori.Add(kategori);
    await db.SaveChangesAsync();

    // Başarılı bir şekilde eklenen yönetici döndürülür
    return Results.Created($"/kategori/{kategori.Id}", kategori);
}).WithTags("Kategori");

app.MapGet("/kategori/bul", async (UygulamaDB db, string? ad) =>
{
    var kategoriler = string.IsNullOrEmpty(ad)
        ? await db.Kategori.ToListAsync()
        : await db.Kategori
            .Where(x => x.Ad.Contains(ad))
            .ToListAsync();

    return kategoriler.Any() ? Results.Ok(kategoriler) : Results.NoContent();
}).WithTags("Kategori");

app.MapGet("/kategori/listele", async (UygulamaDB db) =>
{
    var kategoriler = await db.Kategori.ToListAsync();
    return kategoriler.Any() ? Results.Ok(kategoriler) : Results.NoContent();
}).WithTags("Kategori");

app.MapPut("/kategori/guncelle/{id}", async (UygulamaDB db, int id, Kategori kategori) =>
{
    if (id != kategori.Id)
        return Results.BadRequest("ID uyuşmuyor.");

    var mevcutKategori = await db.Kategori.FindAsync(id);

    if (mevcutKategori is null)
        return Results.NotFound();

    // Yalnızca güncellenebilir alanlar değiştiriliyor
    mevcutKategori.Ad = kategori.Ad;

    try
    {
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (DbUpdateException)
    {
        return Results.StatusCode(500);
    }
}).WithTags("Kategori");

app.MapDelete("/kategori/sil/{id}", async (UygulamaDB db, int id) =>
{
    var kategori = await db.Kategori.FindAsync(id);

    if (kategori == null)
        return Results.NotFound(); // Yönetici bulunamazsa 404 döner

    db.Kategori.Remove(kategori);
    await db.SaveChangesAsync();

    return Results.Ok($"Kategori ID {id} başarıyla silindi."); // Başarılı olursa 200 döner
}).WithTags("Kategori");

#endregion kategori

#region puan/yorum_Film

// GET: /api/yorum?filmId=3
app.MapGet("/yorum/film", async (int filmId, UygulamaDB db) =>
{
    try
    {
        var yorumlar = await db.YorumFilm
            .Where(y => y.FilmId == filmId)
            .OrderByDescending(y => y.Tarih ?? DateTime.MinValue)
            .ToListAsync();

        return Results.Ok(yorumlar);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Yorumlar alınırken hata oluştu: {ex.Message}");
    }
}).WithTags("YorumFilm");
// POST: /api/yorum (MAUI uygulaması bunu çağırıyor)
app.MapPost("/yorum/film", async (YorumFilm yorum, UygulamaDB db) =>
{
    try
    {
        // Tarih ataması
        yorum.Tarih ??= DateTime.Now;

        // Aynı kullanıcı ve film için daha önce "Yıldızlandı" metniyle gönderilmiş puan yorumu varsa sil
        var eskiPuanYorum = await db.YorumFilm
            .FirstOrDefaultAsync(y => y.KullaniciId == yorum.KullaniciId
                                   && y.FilmId == yorum.FilmId
                                   && y.YorumMetni == "Yıldızlandı");

        if (eskiPuanYorum != null)
        {
            db.YorumFilm.Remove(eskiPuanYorum);
        }

        // Kullanıcının diğer yorumlarındaki puanları güncelle (YorumMetni farklı olanlar)
        var digerYorumlar = await db.YorumFilm
            .Where(y => y.KullaniciId == yorum.KullaniciId && y.FilmId == yorum.FilmId && y.YorumMetni != "Yıldızlandı")
            .ToListAsync();

        foreach (var dYorum in digerYorumlar)
        {
            dYorum.Puan = yorum.Puan;
        }

        // Yeni puan yorumunu ekle
        db.YorumFilm.Add(yorum);

        await db.SaveChangesAsync();

        return Results.Ok("Yorum başarıyla eklendi ve puanlar güncellendi.");
    }
    catch (DbUpdateException dbEx)
    {
        return Results.Problem($"Veritabanı hatası: {dbEx.InnerException?.Message ?? dbEx.Message}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Beklenmeyen hata: {ex.Message}");
    }
}).WithTags("YorumFilm");

app.MapDelete("/yorum/film", async (int id, UygulamaDB db) =>
{
    try
    {
        var yorum = await db.YorumFilm.FindAsync(id);
        if (yorum == null)
            return Results.NotFound("Yorum bulunamadı.");

        db.YorumFilm.Remove(yorum);
        await db.SaveChangesAsync();

        return Results.Ok("Yorum silindi.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Yorum silinirken hata oluştu: {ex.Message}");
    }
}).WithTags("YorumFilm");

#endregion puan/yorum_Film

#region puan/yorum_Dizi

// GET: /api/yorum?diziId=3
app.MapGet("/yorum/dizi", async (int diziId, UygulamaDB db) =>
{
    try
    {
        var yorumlar = await db.YorumDizi
            .Where(y => y.DiziId == diziId)
            .OrderByDescending(y => y.Tarih ?? DateTime.MinValue)
            .ToListAsync();

        return Results.Ok(yorumlar);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Yorumlar alınırken hata oluştu: {ex.Message}");
    }
}).WithTags("YorumDizi");
// POST: /api/yorum (MAUI uygulaması bunu çağırıyor)
app.MapPost("/yorum/dizi", async (YorumDizi yeniYorum, UygulamaDB db) =>
{
    try
    {
        yeniYorum.Tarih ??= DateTime.Now;

        // Aynı kullanıcı, aynı dizi ve "Yıldızlandı" metnine sahip eski puan yorumu varsa sil
        var eskiYildizliyorum = await db.YorumDizi
            .FirstOrDefaultAsync(y => y.DiziId == yeniYorum.DiziId
                                   && y.KullaniciId == yeniYorum.KullaniciId
                                   && y.YorumMetni == "Yıldızlandı");

        if (eskiYildizliyorum != null)
        {
            db.YorumDizi.Remove(eskiYildizliyorum);
            await db.SaveChangesAsync();
        }

        // Aynı kullanıcı ve diziye ait diğer yorumların puanlarını güncelle
        var digerYorumlar = await db.YorumDizi
            .Where(y => y.DiziId == yeniYorum.DiziId
                     && y.KullaniciId == yeniYorum.KullaniciId
                     && y.YorumMetni != "Yıldızlandı")
            .ToListAsync();

        foreach (var yorum in digerYorumlar)
        {
            yorum.Puan = yeniYorum.Puan;
        }

        await db.SaveChangesAsync();

        // Yeni yıldız puan yorumunu ekle
        db.YorumDizi.Add(yeniYorum);
        await db.SaveChangesAsync();

        return Results.Ok("Puan ve yorumlar başarıyla güncellendi.");
    }
    catch (DbUpdateException dbEx)
    {
        return Results.Problem($"Veritabanı hatası: {dbEx.InnerException?.Message ?? dbEx.Message}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Beklenmeyen hata: {ex.Message}");
    }
}).WithTags("YorumDizi");


#endregion puan/yorum_Dizi

#region arama

app.MapGet("/arama", async (string ad, UygulamaDB db) =>
{
    if (string.IsNullOrWhiteSpace(ad))
        return Results.BadRequest("Arama kelimesi boş olamaz.");

    // Normalize helper
    string Normalize(string text) =>
        text.ToLower()
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u");

    // Levenshtein helper
    int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        int[,] d = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) d[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[s.Length, t.Length];
    }

    string normalizedQuery = Normalize(ad);

    // Veritabanından filmleri ve dizileri çekiyoruz
    var filmler = await db.Film.Include(f => f.Kategori).ToListAsync();
    var diziler = await db.Dizi.Include(d => d.Kategori).ToListAsync();

    // Film eşleşmeleri
    var eslesenFilmler = filmler
        .Where(f =>
        {
            string normalizedAd = Normalize(f.Ad);
            return normalizedAd.Contains(normalizedQuery) ||
                   LevenshteinDistance(normalizedAd, normalizedQuery) <= 2;
        })
        .Select(f => new Arama
        {
            Id = f.Id,
            Ad = f.Ad,
            Aciklama = f.Aciklama,
            KategoriAd = f.Kategori?.Ad ?? "Bilinmiyor",
            ImageUrl = f.ImageUrl,
            Tur = TurEnum.Film // 👈 EKLENDİ
        });

    // Dizi eşleşmeleri
    var eslesenDiziler = diziler
        .Where(d =>
        {
            string normalizedAd = Normalize(d.Ad);
            return normalizedAd.Contains(normalizedQuery) ||
                   LevenshteinDistance(normalizedAd, normalizedQuery) <= 2;
        })
        .Select(d => new Arama
        {
            Id = d.Id,
            Ad = d.Ad,
            Aciklama = d.Aciklama,
            KategoriAd = d.Kategori?.Ad ?? "Bilinmiyor",
            ImageUrl = d.ImageUrl,
            Tur = TurEnum.Dizi // 👈 EKLENDİ
        });


    var sonuc = eslesenFilmler.Concat(eslesenDiziler).ToList();

    return Results.Ok(sonuc);
}).WithTags("Arama");

app.MapGet("/arama/kategoriye-gore", async (string kategoriAd, UygulamaDB db) =>
{
    if (string.IsNullOrWhiteSpace(kategoriAd))
        return Results.BadRequest("Kategori adı boş olamaz.");

    var normalizedKategori = kategoriAd.ToLower();

    var filmler = await db.Film
        .Include(f => f.Kategori)
        .Where(f => f.Kategori.Ad.ToLower() == normalizedKategori)
        .Select(f => new Arama
        {
            Id = f.Id,
            Ad = f.Ad,
            Aciklama = f.Aciklama,
            ImageUrl = f.ImageUrl,
            KategoriAd = f.Kategori.Ad,
            Tur = TurEnum.Film
        })
        .ToListAsync();

    var diziler = await db.Dizi
        .Include(d => d.Kategori)
        .Where(d => d.Kategori.Ad.ToLower() == normalizedKategori)
        .Select(d => new Arama
        {
            Id = d.Id,
            Ad = d.Ad,
            Aciklama = d.Aciklama,
            ImageUrl = d.ImageUrl,
            KategoriAd = d.Kategori.Ad,
            Tur = TurEnum.Dizi
        })
        .ToListAsync();

    var sonuc = filmler.Concat(diziler).ToList();

    return Results.Ok(sonuc);
}).WithTags("Arama");


#endregion arama

# region DiziBolumPuanYorum

app.MapPost("/bolumyorumlar", async (DiziBolumYorumDto dto, UygulamaDB db) =>
{
    var yeniYorum = new DiziBolumYorumPuan
    {
        DiziBolumId = dto.DiziBolumId,
        KullaniciId = (int) dto.KullaniciId,
        Puan = dto.Puan,
        Yorum = dto.Yorum,
         
    };

    db.DiziBolumYorumPuan.Add(yeniYorum);
    await db.SaveChangesAsync();

    // Ortalama hesapla ve güncelle
    var puanlar = await db.DiziBolumYorumPuan
        .Where(p => p.DiziBolumId == dto.DiziBolumId)
        .ToListAsync();

    double ortalama = puanlar.Any() ? puanlar.Average(p => p.Puan) : 0;

    var bolum = await db.DiziBolum.FindAsync(dto.DiziBolumId);
    if (bolum is not null)
    {
        bolum.ortalamaPuan = ortalama;
        await db.SaveChangesAsync();
    }

    return Results.Created($"/bolumyorumlar/{yeniYorum.Id}", new
    {
        Id = yeniYorum.Id,
        DiziBolumId = yeniYorum.DiziBolumId,
        KullaniciId = yeniYorum.KullaniciId,
        Puan = yeniYorum.Puan,
        Yorum = yeniYorum.Yorum,
        
    });

}).WithTags("Bolum Yorum Puan");

app.MapGet("/bolumyorumlar/{bolumId:int}", async (int bolumId, UygulamaDB db) =>
{
    var yorumlar = await db.DiziBolumYorumPuan
        .Where(y => y.DiziBolumId == bolumId)
        .Include(y => y.Kullanici)
        .OrderByDescending(y => y.YorumTarihi)

        .Select(y => new
        {
            KullaniciAdi = y.Kullanici.KullaniciAdi,
            YorumMetni = y.Yorum,
            Puan = y.Puan,
            YorumTarihi = y.YorumTarihi
        })
        .ToListAsync();

    return Results.Ok(yorumlar);
}).WithTags("Bolum Yorum Puan");

# endregion DiziBolumPuanYorum

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();



