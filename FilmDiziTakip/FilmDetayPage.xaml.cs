using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Maui.Controls;
using FilmDiziTakip.Modeller;
using CommunityToolkit.Maui.Views;
using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Core.Primitives;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace FilmDiziTakip;

public partial class FilmDetayPage : ContentPage
{
    private int _filmId;
    private int? _kullaniciId;
    private int _kategoriId;
    private int _secilenPuan = 0;
    private readonly HttpClient _httpClient = new();
    private TimeSpan? _kalanPozisyon = null;


    public FilmDetayPage(int filmId, int? kullaniciId)
    {
        InitializeComponent();
        
        _filmId = filmId;
        _kullaniciId = kullaniciId;
        
        InitYildizEtkinlikleri();
        YukleFilmVeYorumlarAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
    }

    private async void YukleFilmVeYorumlarAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync($"https://localhost:7248/film/bul/{_filmId}");
            var film = JsonConvert.DeserializeObject<Film>(response);
            if (film == null) return;

            FilmAdiLabel.Text = film.Ad;
            FilmAciklamaLabel.Text = film.Aciklama;
            _kategoriId = film.KategoriId;

            string videoPath = Path.Combine(@"C:\Users\alpak\OneDrive\Masaüstü\proje\FilmDiziTakip\Resources\Filmler\",
                                            $"{Regex.Replace(film.Ad, $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", "_")}.mp4");

            var mediaSource = !string.IsNullOrWhiteSpace(film.FilmUrl)
                               ? MediaSource.FromUri(film.FilmUrl)
                               : File.Exists(videoPath)
                                 ? MediaSource.FromFile(videoPath)
                                 : throw new FileNotFoundException($"Video bulunamadı: {videoPath}");

            FilmOynatici.Source = mediaSource;

            //FilmOynatici.MediaOpened += async (s, e) =>
            //{
            //    try
            //    {
            //        var filmJson = await _httpClient.GetStringAsync($"https://localhost:7248/film/bul/{_filmId}");
            //        var film = JsonConvert.DeserializeObject<Film>(filmJson);

            //        string videoPath = Path.Combine(@"D:\Users\HakanIslam\Desktop\proje\FilmDiziTakip\Resources\Filmler\",
            //            $"{Regex.Replace(film.Ad, $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", "_")}.mp4");

            //        var mediaSource = !string.IsNullOrWhiteSpace(film.FilmUrl)
            //                          ? MediaSource.FromUri(film.FilmUrl)
            //                          : File.Exists(videoPath)
            //                            ? MediaSource.FromFile(videoPath)
            //                            : throw new FileNotFoundException($"Video bulunamadı: {videoPath}");

            //        // Source'ı yeniden ayarla
            //        FilmOynatici.Source = mediaSource;

            //        // Kaynağı güncelledikten sonra tekrar MediaOpened tetiklenebilir, dikkatli ol.
            //        // Bu kod sonsuz döngüye girmez çünkü genelde aynı Source atanırsa tetiklenmez.                                                                        Video devam etmek için denediğim kod belki işine yarar diye silmedim

            //        // Süre kontrolü
            //        TimeSpan? duration = FilmOynatici.Duration;

            //        if (duration.HasValue && duration.Value.TotalSeconds > 0)
            //        {
            //            var kalanSure = await GetKalanSureFromApiAsync(_kullaniciId, _filmId);

            //            if (kalanSure > TimeSpan.FromSeconds(10) && kalanSure < duration.Value)
            //            {
            //                var cevap = await DisplayActionSheet(
            //                    $"Kaldığın yer: {Math.Floor(kalanSure.TotalSeconds)} sn",
            //                    "İptal", null, "Kaldığım Yerden Devam Et", "Baştan Başla");

            //                if (cevap == "Kaldığım Yerden Devam Et")
            //                {
            //                    await FilmOynatici.SeekTo(kalanSure);
            //                }
            //            }
            //        }

            //        FilmOynatici.Play();
            //    }
            //    catch (Exception ex)
            //    {
            //        await DisplayAlert("MediaOpened Hatası", ex.Message, "Tamam");
            //        FilmOynatici.Play();
            //    }
            //};




            // Diğer işlemler (benzer filmler, yorumlar, puanlar)
            var benzerJson = await _httpClient.GetStringAsync($"https://localhost:7248/film/kategoriye_gore_listele/{_kategoriId}");
            var benzerFilmler = JsonConvert.DeserializeObject<List<Film>>(benzerJson)?.Where(f => f.Id != _filmId).ToList() ?? new();
            BenzerFilmlerList.ItemsSource = benzerFilmler;

            var yorumlar = await YorumlariGetirAsync(_filmId);
            YorumlarList.ItemsSource = yorumlar;

            // Ortalama puanı hesapla
            double ortalamaPuan = 0;
            if (yorumlar.Any())
            {
                ortalamaPuan = yorumlar.Average(y => y.Puan);
            }
            GuncelleOrtalamaPuanGorunumu(ortalamaPuan);


            var rawYorumlar = JsonConvert.DeserializeObject<List<YorumFilm>>(
                await _httpClient.GetStringAsync($"https://localhost:7248/yorum/film?filmId={_filmId}"));

            var mevcutPuan = rawYorumlar?.FirstOrDefault(y => y.KullaniciId == _kullaniciId)?.Puan;

            if (mevcutPuan > 0)
            {
                _secilenPuan = mevcutPuan.Value;
                GuncelleYildizGorunumu(_secilenPuan);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Yükleme sırasında bir hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private void InitYildizEtkinlikleri()
    {
        foreach (var view in YildizPanel.Children.OfType<Label>())
        {
            view.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => YildizTiklandi(view))
            });
        }
    }

    private async void YildizTiklandi(Label tiklanan)
    {
        if (!int.TryParse(tiklanan.ClassId, out int puan)) return;

        _secilenPuan = puan;
        GuncelleYildizGorunumu(puan);

        var yeniYorum = new YorumFilm
        {
            FilmId = _filmId,
            KullaniciId = _kullaniciId,
            Puan = puan,
            YorumMetni = "Yıldızlandı"
        };

        var response = await _httpClient.PostAsync("https://localhost:7248/yorum/film",
            new StringContent(JsonConvert.SerializeObject(yeniYorum), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            BildirimLabel.Text = "Puan başarıyla gönderildi.";
            BildirimLabel.IsVisible = true;
            await Task.Delay(2000);
            BildirimLabel.IsVisible = false;
            YukleFilmVeYorumlarAsync();
        }
        else
        {
            await DisplayAlert("Hata", "Puan gönderilemedi.", "Tamam");
        }
    }

    private void GuncelleYildizGorunumu(int puan)
    {
        foreach (var label in YildizPanel.Children.OfType<Label>())
        {
            if (int.TryParse(label.ClassId, out int index))
            {
                label.TextColor = index <= puan ? Colors.Gold : Colors.Gray;
                label.Text = "★";
            }
        }
    }

    private async void GonderYorumClicked(object sender, EventArgs e)
    {
        var metin = KullaniciYorumEditor.Text?.Trim();
        if (string.IsNullOrWhiteSpace(metin))
        {
            await DisplayAlert("Uyarı", "Lütfen yorum metni giriniz.", "Tamam");
            return;
        }

        var yorum = new YorumFilm
        {
            FilmId = _filmId,
            KullaniciId = _kullaniciId,
            YorumMetni = metin,
            Puan = _secilenPuan == 0 ? 3 : _secilenPuan
        };

        var response = await _httpClient.PostAsync("https://localhost:7248/yorum/film",
            new StringContent(JsonConvert.SerializeObject(yorum), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            KullaniciYorumEditor.Text = string.Empty;
            await DisplayAlert("Başarılı", "Yorum gönderildi.", "Tamam");
            YukleFilmVeYorumlarAsync();

        }
        else
        {
            await DisplayAlert("Hata", "Yorum gönderilemedi.", "Tamam");
        }
    }

    private async Task<List<YorumDtoFilm>> YorumlariGetirAsync(int filmId)
    {
        var yorumJson = await _httpClient.GetStringAsync($"https://localhost:7248/yorum/film?filmId={filmId}");
        var yorumlar = JsonConvert.DeserializeObject<List<YorumFilm>>(yorumJson) ?? new();

        var kullaniciJson = await _httpClient.GetStringAsync("https://localhost:7248/kullanici/listele");
        var kullanicilar = JsonConvert.DeserializeObject<List<Kullanici>>(kullaniciJson) ?? new();

        return yorumlar.Select(y =>
        {
            var kullanici = kullanicilar.FirstOrDefault(k => k.Id == y.KullaniciId);
            return new YorumDtoFilm
            {
                YorumMetni = y.YorumMetni,
                Puan = y.Puan,
                KullaniciAdi = kullanici?.KullaniciAdi ?? "Bilinmiyor",
                Tarih = y.Tarih,
            };
        }).ToList();
    }

    private async void BenzerFilmeGit(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Film secilenFilm)
        {
            await Navigation.PushAsync(new FilmDetayPage(secilenFilm.Id, _kullaniciId));
            BenzerFilmlerList.SelectedItem = null;
        }
    }

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new KullaniciMainPage(_kullaniciId));

        var sure = FilmOynatici.Position.TotalSeconds;
       
        await GuncelleKalanSure(_kullaniciId.Value, _filmId, sure);
    }

    private async void CikisYap(object sender, EventArgs e)
    {
        // Kullanıcıya çıkış yaptığını bildir
        await DisplayAlert("Çıkış", "Çıkış yaptınız. Lütfen tekrar giriş yapmak için bilgilerinizi giriniz.", "Tamam");
        
       
        // MainPage'e yönlendirme (örneğin giriş sayfası)
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }


    private void OnStopClicked(object sender, EventArgs e)
    {
        if (FilmOynatici != null)
        {
            double kalanSure = FilmOynatici.Position.TotalSeconds;
            var key = $"LastPlaybackPosition_{_kullaniciId}_{_filmId}";
            Preferences.Set(key, kalanSure);
        }
    }

    public async Task GuncelleKalanSure(int kullaniciId, int filmId, double kalanSure)                                                      ///
    {
        var model = new { KullaniciId = kullaniciId, FilmId = filmId, KalanSure = kalanSure };
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");                                 // bura da süreyi kaydediyo adam akıllı eklemesini istiyosan filmin olduğu sayfadan geri dön tuşunu kullanarak çık                          

        var response = await _httpClient.PostAsync("https://localhost:7248/izleme-durumu/ekle", content);

        if (!response.IsSuccessStatusCode)                                                                                                   ///
        {
            await DisplayAlert("Hata", "Kalan süre güncellenemedi.", "Tamam");
        }
    }

    private async Task<TimeSpan> GetKalanSureFromApiAsync(int? kullaniciId, int filmId)
    {
        try
        {
            using var httpClient = new HttpClient();
            string url = $"https://localhost:7248/izleme-durumu/kalan-sure?kullaniciId={kullaniciId}&filmId={filmId}";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API hatası: {response.StatusCode}");
                return TimeSpan.Zero;
            }                                                                                                                                                    // bura da kaldığın süreyi veritabanından çekmeye yarıyo bu çalışıyo sıkıntısız

            var content = await response.Content.ReadAsStringAsync();
            if (double.TryParse(content, out double kalanSure))
            {
                return TimeSpan.FromSeconds(kalanSure);
            }

            Console.WriteLine("Kalan süre parse edilemedi.");
            return TimeSpan.Zero;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API çağrısı sırasında hata: {ex.Message}");
            return TimeSpan.Zero;
        }
    }

    private void GuncelleOrtalamaPuanGorunumu(double ortalamaPuan)
    {
        int tamKisim = (int)Math.Floor(ortalamaPuan);
        bool yarimYildizVar = (ortalamaPuan - tamKisim) >= 0.5;

        for (int i = 0; i < OrtalamaPuanPanel.Children.Count; i++)
        {
            if (OrtalamaPuanPanel.Children[i] is Label yildiz)
            {
                if (i < tamKisim)
                {
                    yildiz.Text = "★";
                    yildiz.TextColor = Colors.Gold;
                }
                else if (i == tamKisim && yarimYildizVar)
                {
                    yildiz.Text = "⯪"; // Yaklaşık yarım yıldız göstermek için farklı karakter (buraya istersen farklı bir karakter koyabilirsin)
                    yildiz.TextColor = Colors.Gold;
                }
                else
                {
                    yildiz.Text = "☆";
                    yildiz.TextColor = Colors.Gray;
                }
            }
        }
    }


    public class YorumDtoFilm
    {
        public string YorumMetni { get; set; }
        public int Puan { get; set; }
        public string KullaniciAdi { get; set; }
        public DateTime? Tarih { get; set; }
    }
}
