using System.Net.Http.Json;
using System.Text.Json;
using FilmDiziTakip.Servisler;
using FilmDiziTakip.Modeller;
using System.Collections.ObjectModel;
using System.IO;

namespace FilmDiziTakip;

public partial class KullaniciMainPage : ContentPage
{
    private readonly IDiziService _diziServisi = new DiziServisi();
    private Dictionary<int, string> kategoriDict = new();
    private int? _kullaniciId;
    List<string> bugunEklenenler = new();

    public static int? KullaniciId { get; set; }

    public KullaniciMainPage(int? KullaniciIdsi)
    {
        InitializeComponent();
        _kullaniciId = KullaniciIdsi;
    }

   

    private async void CikisYap(object sender, EventArgs e)
    {
        // Kullanıcıya çıkış yaptığını bildir
        await DisplayAlert("Çıkış", "Çıkış yaptınız. Lütfen tekrar giriş yapmak için bilgilerinizi giriniz.", "Tamam");

        // MainPage'e yönlendirme (örneğin giriş sayfası)
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await KategorileriYukle();
            
            var bugunEklenenler = new List<string>();

            var tumFilmler = await FilmleriYukle(bugunEklenenler);
            var tumDiziler = await DizileriYukle(bugunEklenenler);

            // burada birleştirme yapabilirsiniz
            var hepsi = tumFilmler.Concat(tumDiziler).ToList();

            // ortak bildirim
           


            if ((tumFilmler?.Any() ?? false) || (tumDiziler?.Any() ?? false))
            {
                var filmKategorileri = tumFilmler
                    .Where(f => f.KategoriAd != null)
                    .GroupBy(f => f.KategoriAd)
                    .Select(g => new KategoriGrup
                    {
                        KategoriAdi = g.Key,
                        Filmler = g.ToList()
                    }).ToList();

                var diziKategorileri = tumDiziler
                    .Where(d => d.KategoriAd != null)
                    .GroupBy(d => d.KategoriAd)
                    .Select(g => new KategoriGrup
                    {
                        KategoriAdi = g.Key,
                        Filmler = g.ToList()
                    }).ToList();

                var filmDiziGruplari = new List<FilmDiziGrup>
                {
                    new FilmDiziGrup { GrupAdi = "Filmler", Kategoriler = filmKategorileri },
                    new FilmDiziGrup { GrupAdi = "Diziler", Kategoriler = diziKategorileri }
                };

                kategoriGrupluListView.ItemsSource = filmDiziGruplari;
            }
            else
            {
                await DisplayAlert("Uyarı", "Hiç film veya dizi bulunamadı.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Beklenmeyen bir hata oluştu:\n{ex.Message}", "Tamam");
        }
    }

    private async Task<List<FilmDizi>> FilmleriYukle(List<string> bugunEklenenler)
    {
        var sonuc = new List<FilmDizi>();
        try
        {
            HttpClient client = new();
            var response = await client.GetAsync("https://localhost:7248/film/listele");

            if (!response.IsSuccessStatusCode)
                return sonuc;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var filmler = JsonSerializer.Deserialize<List<Film>>(json, options);

            foreach (var film in filmler)
            {
                bool bugunEklendi = film.CikisTarihi.Date == DateTime.Today;

                if (!string.IsNullOrWhiteSpace(film.ImageUrl) && film.ImageUrl.StartsWith("http"))
                {
                    string localPath = await DownloadImageAsync(film.ImageUrl, film.Ad ?? $"film_{film.Id}", true);

                    if (!string.IsNullOrWhiteSpace(localPath) && File.Exists(localPath))
                    {
                        film.ImageUrl = localPath;
                        film.CikisTarihi = film.CikisTarihi == DateTime.MinValue ? DateTime.Today : film.CikisTarihi;
                        film.Ad ??= "Bilinmeyen";

                        await client.PutAsJsonAsync($"https://localhost:7248/film/guncelle/{film.Id}", film);
                    }
                }

                if (bugunEklendi)
                    bugunEklenenler.Add($"🎬 {film.Ad ?? "Bilinmeyen"}");

                sonuc.Add(new FilmDizi
                {
                    Id = film.Id,
                    Ad = film.Ad,
                    ImageUrl = film.ImageUrl,
                    KategoriAd = kategoriDict.GetValueOrDefault(film.KategoriId, "Bilinmeyen"),
                    IsFilm = true
                });
            }
        }
        catch { }

        return sonuc;
    }

    private async Task<List<FilmDizi>> DizileriYukle(List<string> bugunEklenenler)
    {
        var sonuc = new List<FilmDizi>();
        try
        {
            HttpClient client = new();
            var response = await client.GetAsync("https://localhost:7248/dizi/listele");

            if (!response.IsSuccessStatusCode)
                return sonuc;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var diziler = JsonSerializer.Deserialize<List<Dizi>>(json, options);

            foreach (var dizi in diziler)
            {
                bool bugunEklendi = dizi.YuklenmeTarihi.Date == DateTime.Today;

                if (!string.IsNullOrWhiteSpace(dizi.ImageUrl) && dizi.ImageUrl.StartsWith("http"))
                {
                    string localPath = await DownloadImageAsync(dizi.ImageUrl, dizi.Ad ?? $"dizi_{dizi.Id}", false);

                    if (!string.IsNullOrWhiteSpace(localPath) && File.Exists(localPath))
                    {
                        dizi.ImageUrl = localPath;
                        dizi.YayınTarihi = dizi.YayınTarihi == DateTime.MinValue ? DateTime.Today : dizi.YayınTarihi;
                        dizi.Ad ??= "Bilinmeyen";

                        await client.PutAsJsonAsync($"https://localhost:7248/dizi/guncelle/{dizi.Id}", dizi);
                    }
                }

                if (bugunEklendi)
                    bugunEklenenler.Add($"📺 {dizi.Ad ?? "Bilinmeyen"}");

                sonuc.Add(new FilmDizi
                {
                    Id = dizi.Id,
                    Ad = dizi.Ad,
                    ImageUrl = dizi.ImageUrl,
                    KategoriAd = kategoriDict.GetValueOrDefault(dizi.KategoriId, "Bilinmeyen"),
                    IsFilm = false
                });
            }
        }
        catch { }

        return sonuc;
    }


    public async Task<string> DownloadImageAsync(string imageUrl, string name, bool isFilm)
    {
        try
        {
            using HttpClient client = new();
            var bytes = await client.GetByteArrayAsync(imageUrl);
            string safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
            string folderPath = isFilm ?
                @"C:\Users\alpak\OneDrive\Masaüstü\proje\FilmDiziTakip\Resources\Images\Dizi\\" :
                @"C:\Users\alpak\OneDrive\Masaüstü\proje\FilmDiziTakip\Resources\Images\Film\\";
            Directory.CreateDirectory(folderPath);
            string localPath = Path.Combine(folderPath, $"{safeName}.jpg");

            if (!File.Exists(localPath))
                await File.WriteAllBytesAsync(localPath, bytes);

            return localPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DownloadImageAsync] Hata: {ex.Message}");
            return string.Empty;
        }
    }

    private async void SifreDegistir_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SifreDegistirPage());
    }

    private async Task KategorileriYukle()
    {
        try
        {
            HttpClient client = new();
            string url = "https://localhost:7248/kategori/listele";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Uyarı", "Kategori API erişilemiyor.", "Tamam");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var kategoriler = JsonSerializer.Deserialize<List<Kategori>>(json, options);

            kategoriDict = kategoriler.ToDictionary(k => k.Id, k => k.Ad);
            kategoriListView.ItemsSource = kategoriler;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Kategori yüklenemedi: {ex.Message}", "Tamam");
        }
    }

    private async void KategoriListele(object sender, EventArgs e)
    {
        bool yeniDurum = !kategoriListView.IsVisible;
        kategoriListView.IsVisible = yeniDurum;
        kategoriBaslikFrame.IsVisible = yeniDurum;
        kategoriGosterBtn.Text = yeniDurum ? "Kategorileri Gizle" : "Kategorileri Göster";

        if (yeniDurum && kategoriListView.ItemsSource == null)
            await KategorileriYukle();
    }

    private async void KategoriSecildi(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Kategori secilenKategori)
        {
            await Navigation.PushAsync(new KategoriDetayPage(secilenKategori,_kullaniciId));
            kategoriListView.SelectedItem = null;
        }
    }

    private async void FilmDiziSecildi(object sender, SelectionChangedEventArgs e)
    {
        var secilen = e.CurrentSelection.FirstOrDefault() as FilmDizi;
        if (secilen == null) return;

        if (secilen.IsFilm == true) // Film ise
        {
            await Navigation.PushAsync(new FilmDetayPage(secilen.Id, _kullaniciId));
        }
        else if (secilen.IsFilm == false) // Dizi ise
        {
            await Navigation.PushAsync(new DiziDetayPage(secilen.Id, _kullaniciId));
        }

    ((CollectionView)sender).SelectedItem = null;
    }




    private async void FilmAraBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilmDiziAramaPage(_kullaniciId));
    }

    private async void GosterimSecildi(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is GosterimItem item)
        {
            if (item.IsFilm)
                await Navigation.PushAsync(new FilmDetayPage(item.Id,_kullaniciId));
            else
                await Navigation.PushAsync(new DiziDetayPage(item.Id,_kullaniciId));

            (sender as CollectionView).SelectedItem = null;
        }
    }
    private async void IzlemeGecmisi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new IzlemeGecmisiPage(_kullaniciId)); // Sayfa varsa
    }




    public class KategoriGrupluGosterim : ObservableCollection<GosterimItem>
    {
        public string KategoriAd { get; private set; }

        public KategoriGrupluGosterim(string kategoriAd, IEnumerable<GosterimItem> gosterimler) : base(gosterimler)
        {
            KategoriAd = kategoriAd;
        }
    }

}