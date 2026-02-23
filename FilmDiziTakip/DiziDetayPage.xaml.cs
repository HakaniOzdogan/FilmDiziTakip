using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using FilmDiziTakip.Modeller;
using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace FilmDiziTakip;

public partial class DiziDetayPage : ContentPage
{
    private int _diziId;
    private int? _kullaniciId;
    private int _kategoriId;
    private int _secilenPuan = 0;
    public readonly Dizi secilenDizi;
    private readonly HttpClient _httpClient = new();
    public ObservableCollection<DiziSezon> Sezonlar { get; set; } = new();
    public ObservableCollection<DiziBolum> Bolumler { get; set; } = new();


    public DiziDetayPage(int diziId,int? KullaniciIdsi)
    {
        InitializeComponent();
        _kullaniciId = KullaniciIdsi;
        _diziId = diziId;
        String Diiizi = _diziId.ToString();
        BindingContext = this;
        YukleSezonlarAsync();

        InitYildizEtkinlikleri();
        YukleDiziVeYorumlarAsync();
    }

    private async void YukleSezonlarAsync()
    {
        var json = await _httpClient.GetStringAsync($"https://localhost:7248/api/diziler/{_diziId}/sezonlar");
        var sezonlar = JsonConvert.DeserializeObject<List<DiziSezon>>(json);
        Sezonlar.Clear();
        foreach (var sezon in sezonlar)
            Sezonlar.Add(sezon);
    }

    private async void SezonlarListesi_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is DiziSezon secilenSezon)
        {
            var json = await _httpClient.GetStringAsync($"https://localhost:7248/api/sezonlar/{secilenSezon.Id}/bolumler");
            var bolumler = JsonConvert.DeserializeObject<List<DiziBolum>>(json);
            Bolumler.Clear();
            foreach (var bolum in bolumler)
                Bolumler.Add(bolum);
        }
    }

    private async void BenzerDiziyeGit(object sender, SelectionChangedEventArgs e)
    {
        try
        {


            if (e.CurrentSelection.FirstOrDefault() is Dizi secilenDizi)
            {
                

                
                await Navigation.PushAsync(new DiziDetayPage(secilenDizi.Id,_kullaniciId));

                BenzerDizilerList.SelectedItem = null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Sayfa açılırken bir hata oluştu:  {ex.Message}", "Tamam");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Medyayı durdur
        if (DiziOynatici != null && DiziOynatici.CurrentState == MediaElementState.Playing)
        {
            DiziOynatici.Stop();
        }
    }

    private async void YukleDiziVeYorumlarAsync()

    {
        try
        {
            // 🎥 1. Dizi verisini çek
            var response = await _httpClient.GetStringAsync($"https://localhost:7248/dizi/bul/{_diziId}");
            var dizi = JsonConvert.DeserializeObject<Dizi>(response);
            if (dizi == null) return;

            // 🎬 2. Dizi adı ve türü label'lara yaz
            DiziAdiLabel.Text = dizi.Ad;
            DiziAciklamaLabel.Text = dizi.Aciklama;
            _kategoriId = dizi.KategoriId;
            
            // 🧹 3. Geçerli dosya adı oluştur
            string temizAd = Regex.Replace(dizi.Ad, $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", "_");
            string dosyaAdi = $"{temizAd}.mp4";
            string videoPath = Path.Combine(@"C:\Users\alpak\OneDrive\Masaüstü\proje\FilmDiziTakip\Resources\Diziler\", dosyaAdi);

            // 📂 4. Önce FilmUrl kullan, yoksa yerel dosya kontrolü yap
            if (!string.IsNullOrWhiteSpace(dizi.DiziUrl))
            {

                DiziOynatici.Source = MediaSource.FromUri(dizi.DiziUrl);
            }
            else if (File.Exists(videoPath))
            {
                DiziOynatici.Source = MediaSource.FromFile(videoPath);
            }
            else
            {
                await DisplayAlert("Hata", $"Video bulunamadı: {videoPath}", "Tamam");
            }

            // 🎞️ 5. Benzer filmleri getir
            var benzer = await _httpClient.GetStringAsync($"https://localhost:7248/dizi/kategoriye_gore_listele/{_kategoriId}");
            var benzerDiziler = JsonConvert.DeserializeObject<List<Dizi>>(benzer)
                                ?.Where(f => f.Id != _diziId).ToList() ?? new();
            
            BenzerDizilerList.ItemsSource = benzerDiziler;

            // 💬 6. Yorumları getir


            int diziId = _diziId; // zaten sende bu değişken var
            var yorumlari = await YorumlariGetirAsync(diziId);
            YorumlarList.ItemsSource = yorumlari;

            

            var yorumJson = await _httpClient.GetStringAsync($"https://localhost:7248/yorum/dizi?diziId={_diziId}");
            var yorumlar = JsonConvert.DeserializeObject<List<YorumDizi>>(yorumJson) ?? new();

            double ortalamaPuan = 0;
            if (yorumlar.Any())
            {
                ortalamaPuan = yorumlar.Average(y => y.Puan);
            }
            GuncelleOrtalamaPuanGorunumu(ortalamaPuan);

            // ⭐ 7. Kullanıcının daha önceki puanı varsa göster
            var mevcutPuan = yorumlar.FirstOrDefault(y => y.KullaniciId == _kullaniciId)?.Puan;
            if (mevcutPuan > 0)
            {
                _secilenPuan = mevcutPuan.Value;
                GuncelleYildizGorunumu(_secilenPuan);
            }
            
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Yükleme sırasında bir sorun oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new KullaniciMainPage(_kullaniciId));
    }

    private async void CikisYap(object sender, EventArgs e)
    {
        // Kullanıcıya çıkış yaptığını bildir
        await DisplayAlert("Çıkış", "Çıkış yaptınız. Lütfen tekrar giriş yapmak için bilgilerinizi giriniz.", "Tamam");

        // MainPage'e yönlendirme (örneğin giriş sayfası)
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }


    private async void YildizTiklandi(object sender)
    {
        var tiklanan = sender as Label;
        if (tiklanan == null) return;

        if (!int.TryParse(tiklanan.ClassId, out int puan)) return;

        _secilenPuan = puan;
        GuncelleYildizGorunumu(puan);

        var yeniYorum = new YorumDizi
        {
            DiziId = _diziId,
            KullaniciId = _kullaniciId,
            Puan = puan,
            YorumMetni = "Yıldızlandı" // İstersen buraya gerçek yorum metni de ekleyebilirsin
        };

        var json = JsonConvert.SerializeObject(yeniYorum);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://localhost:7248/yorum/dizi", content);

        if (response.IsSuccessStatusCode)
        {
            BildirimLabel.Text = "Puan başarıyla gönderildi.";
            BildirimLabel.IsVisible = true;
            await Task.Delay(2000);
            BildirimLabel.IsVisible = false;

            YukleDiziVeYorumlarAsync();
        }
        else
        {
            await DisplayAlert("Hata", "Puan gönderilemedi.", "Tamam");
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

    private void GuncelleYildizGorunumu(int puan)
    {
        foreach (var label in YildizPanel.Children.OfType<Label>())
        {
            if (int.TryParse(label.ClassId, out int index))
            {
                label.TextColor = (index <= puan) ? Colors.Gold : Colors.Gray;
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

        var yorum = new YorumDizi
        {
            DiziId = _diziId,
            KullaniciId = _kullaniciId,
            YorumMetni = metin,
            Puan = _secilenPuan == 0 ? 3 : _secilenPuan
        };

        var content = new StringContent(JsonConvert.SerializeObject(yorum), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://localhost:7248/yorum/dizi", content);
            
        if (response.IsSuccessStatusCode)
        {
            KullaniciYorumEditor.Text = string.Empty;
            await DisplayAlert("Başarılı", "Yorum gönderildi.", "Tamam");
            YukleDiziVeYorumlarAsync();
        }
        else
        {
            await DisplayAlert("Hata", "Yorum gönderilemedi.", "Tamam");
        }

    }

    public static class ImageHelper
    {
        public static async Task<string> DownloadImageAsync(string imageUrl, string diziName)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var bytes = await client.GetByteArrayAsync(imageUrl);

                string safeName = string.Join("_", diziName.Split(Path.GetInvalidFileNameChars()));
                string folderPath = @"D:\Users\HakanIslam\Desktop\proje\FilmDiziTakip\Resources\Images\Dizi\";
                Directory.CreateDirectory(folderPath);

                string localPath = Path.Combine(folderPath, $"{safeName}.jpg");

                if (!File.Exists(localPath))
                {
                    await File.WriteAllBytesAsync(localPath, bytes);
                }

                return localPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DownloadImageAsync] Hata: {ex.Message}");
                return string.Empty;
            }
        }
    }

    private async Task<List<YorumDtoDizi>> YorumlariGetirAsync(int diziId)
    {
        using var httpClient = new HttpClient();

        // 1. Diziye ait yorumları çek
        var yorumResponse = await httpClient.GetAsync($"https://localhost:7248/yorum/dizi?diziId={diziId}");
        var yorumContent = await yorumResponse.Content.ReadAsStringAsync();
        var yorumlar = JsonConvert.DeserializeObject<List<YorumDizi>>(yorumContent);

        // 2. Kullanıcıları çek
        var kullaniciResponse = await httpClient.GetAsync("https://localhost:7248/kullanici/listele");
        var kullaniciContent = await kullaniciResponse.Content.ReadAsStringAsync();
        var kullanicilar = JsonConvert.DeserializeObject<List<Kullanici>>(kullaniciContent);

        // 3. Yorumları kullanıcı adıyla eşleştir
        var yorumDtos = yorumlar.Select(y =>
        {
            var kullanici = kullanicilar.FirstOrDefault(k => k.Id == y.KullaniciId);
            return new YorumDtoDizi
            {
                YorumMetni = y.YorumMetni,
                Puan = y.Puan,
                KullaniciAdi = kullanici?.KullaniciAdi ?? "Bilinmiyor",
                Tarih = y.Tarih,
                
                
            };
        }).ToList();

        return yorumDtos;
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

    public class YorumDtoDizi
    {
        public string YorumMetni { get; set; }
        public int? Puan { get; set; }
        public string KullaniciAdi { get; set; }
        public DateTime? Tarih {  get; set; }

    }

    private async void BolumTapped(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is DiziBolum secilenBolum)
        {
            // Tüm bölümleri al
            var bolumler = ((CollectionView)sender).ItemsSource.Cast<DiziBolum>().ToList();

            var response = await _httpClient.GetStringAsync($"https://localhost:7248/dizi/bul/{_diziId}");
            var dizi = JsonConvert.DeserializeObject<Dizi>(response);
            if (dizi == null) return;

            // Seçilen bölümün index'ini bul
            int secilenIndex = bolumler.IndexOf(secilenBolum);

            // Yeni sayfaya geçiş
            await Navigation.PushAsync(new BolumDetayPage(bolumler, secilenIndex,dizi.Ad,dizi.Id));

            // Seçimi temizle
            ((CollectionView)sender).SelectedItem = null;
        }
    }

}