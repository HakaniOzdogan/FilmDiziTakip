using Newtonsoft.Json;
using System.Text;
using FilmDiziTakip.Modeller;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Net.Http;



namespace FilmDiziTakip;

public partial class DiziTakibiPage : ContentPage
{
	public DiziTakibiPage()
	{

       
        InitializeComponent();
	}

    private List<Kategori> _kategoriler = new();

    private async void YeniDiziEkleClicked(object sender, EventArgs e)
    {
        DiziEklePanel.IsVisible = !DiziEklePanel.IsVisible;
        using var client = new HttpClient();
        if (_kategoriler.Count == 0)
        {
            var response = await client.GetStringAsync("https://localhost:7248/kategori/listele");
            _kategoriler = JsonConvert.DeserializeObject<List<Kategori>>(response) ?? new();
            kategoriPicker.ItemsSource = _kategoriler.Select(k => k.Ad).ToList();
        }
    }

    private async void DiziKaydetClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtAd.Text) || kategoriPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Hata", "Tüm alanları doldurun.", "Tamam");
            return;
        }
        using var client = new HttpClient();
        var secilenKategori = _kategoriler[kategoriPicker.SelectedIndex];

        var dizi = new DiziEkleModel
        {
            Ad = txtAd.Text,
            Aciklama = txtAciklama.Text,
            YayınTarihi = dtYayinTarihi.Date,
            Yonetmen = txtYonetmen.Text,
            Puan = decimal.TryParse(txtPuan.Text, out var puan) ? puan : 0,
            KategoriId = secilenKategori.Id,
            ImageUrl = txtImageUrl.Text
        };

        var json = JsonConvert.SerializeObject(dizi);
        var response = await client.PostAsync("https://localhost:7248/dizi/ekle",
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Başarılı", "Dizi başarıyla eklendi.", "Tamam");
            DiziEklePanel.IsVisible = false;
            txtAd.Text = txtAciklama.Text = txtYonetmen.Text = txtPuan.Text = txtImageUrl.Text = "";
            kategoriPicker.SelectedIndex = -1;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Hata", $"Ekleme başarısız: {error}", "Tamam");
        }
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
            var diziler = System.Text.Json.JsonSerializer.Deserialize<List<Dizi>>(json, options);

            foreach (var dizi in diziler)
            {
               

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

               

                sonuc.Add(new FilmDizi
                {
                    Id = dizi.Id,
                    Ad = dizi.Ad,
                    ImageUrl = dizi.ImageUrl,
                    KategoriAd = dizi.KategoriAd, // artık direkt modelden alıyoruz
                    IsFilm = false
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Diziler yüklenirken hata oluştu: " + ex.Message, "Tamam");
        }

        return sonuc;
    }

    private async void ListeleButton_Clicked(object sender, EventArgs e)
    {
        var bugunEklenenler = new List<string>();
        var diziler = await DizileriYukle(bugunEklenenler);

        // Örneğin CollectionView'e bağlayalım:
        DiziCollectionView.ItemsSource = diziler;

        // Bugün eklenenleri gösterelim
        if (bugunEklenenler.Any())
        {
            string mesaj = string.Join("\n", bugunEklenenler);
            await DisplayAlert("Bugün Eklenenler", mesaj, "Tamam");
        }
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








    private async void DiziCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var secilen = e.CurrentSelection.FirstOrDefault() as FilmDizi; // Modelin ismi senin projenle uyumlu olsun

        if (secilen == null)
            return;

        _seciliDiziId = secilen.Id;

        Console.WriteLine($"Seçilen Dizi: {_seciliDiziId} - {secilen.Ad}");

        // 🔥 Sezonları getir
        await SezonlariYukleAsync(_seciliDiziId);
    }


    private async void DiziSil_Clicked(object sender, EventArgs e)
    {
        if (_seciliDiziId == 0)
        {
            await DisplayAlert("Uyarı", "Lütfen önce bir dizi seçin.", "Tamam");
            return;
        }

        bool onay = await DisplayAlert("Dizi Sil", "Bu diziyi silmek istediğinize emin misiniz?", "Evet", "Hayır");
        if (!onay) return;

        using var client = new HttpClient();
        var response = await client.DeleteAsync($"https://localhost:7248/dizi/{_seciliDiziId}");

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Başarılı", "Dizi silindi.", "Tamam");
            _seciliDiziId = 0;
            DiziCollectionView.ItemsSource = null;
            ListeleButton_Clicked(null, null);
            SezonCollectionView.ItemsSource = null;
            BolumEklePanel.IsVisible = false;
        }
        else
        {
            await DisplayAlert("Hata", "Dizi silinemedi.", "Tamam");
        }
    }




   










    private async void SezonSil_Clicked(object sender, EventArgs e)
    {
        if (_seciliSezon == null)
        {
            await DisplayAlert("Uyarı", "Lütfen bir sezon seçin.", "Tamam");
            return;
        }

        bool onay = await DisplayAlert("Sezon Sil", $"Sezon {_seciliSezon.SezonNo} silinsin mi?", "Evet", "Hayır");
        if (!onay) return;

        using var client = new HttpClient();
        var response = await client.DeleteAsync($"https://localhost:7248/api/sezonlar/{_seciliSezon.Id}");

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Başarılı", "Sezon silindi.", "Tamam");
            _sezonlar.Remove(_seciliSezon);
            SezonCollectionView.ItemsSource = null;
            SezonCollectionView.ItemsSource = _sezonlar;
            _seciliSezon = null;
            BolumEklePanel.IsVisible = false;
        }
        else
        {
            await DisplayAlert("Hata", "Sezon silinemedi.", "Tamam");
        }
    }

    



    // Seçili dizi ve sezonlar
    private int _seciliDiziId;
    private List<DiziSezon> _sezonlar = new();
    private DiziSezon? _seciliSezon;

    public string SelectedSezonLabel => _seciliSezon != null ? $"Yeni Bölüm Ekle - Sezon {_seciliSezon.SezonNo}" : string.Empty;

    // Dizi seçildikten sonra çağrılır, sezonlar yüklenir:
    private async Task SezonlariYukleAsync(int diziId)
    {
        _seciliDiziId = diziId;

        using var client = new HttpClient();

        var res = await client.GetAsync($"https://localhost:7248/api/diziler/{diziId}/sezonlar");
        if (!res.IsSuccessStatusCode)
        {
            await DisplayAlert("Hata", "Sezonlar yüklenemedi", "Tamam");
            return;
        }

        var json = await res.Content.ReadAsStringAsync();
        _sezonlar = System.Text.Json.JsonSerializer.Deserialize<List<DiziSezon>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        SezonCollectionView.ItemsSource = _sezonlar;

        BolumEklePanel.IsVisible = false;
        _seciliSezon = null;
    }

    // Yeni sezon ekle butonuna basıldığında:
    private async void YeniSezonEkle_Clicked(object sender, EventArgs e)
    {
        using var client = new HttpClient();

        // Veritabanından mevcut en yüksek sezon no'yu çek
        var res = await client.GetAsync($"https://localhost:7248/api/diziler/{_seciliDiziId}/sezonlar");
        if (!res.IsSuccessStatusCode)
        {
            await DisplayAlert("Hata", "Sezonlar alınamadı", "Tamam");
            return;
        }
        var json = await res.Content.ReadAsStringAsync();
        var sezonlar = System.Text.Json.JsonSerializer.Deserialize<List<DiziSezon>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        int maxSezonNo = sezonlar.Count > 0 ? sezonlar.Max(s => s.SezonNo) : 0;
        int yeniSezonNo = maxSezonNo + 1;

        var yeniSezon = new { DiziId = _seciliDiziId, SezonNo = yeniSezonNo };

        var ekleResponse = await client.PostAsJsonAsync("https://localhost:7248/api/sezonlar/ekle", yeniSezon);
        if (!ekleResponse.IsSuccessStatusCode)
        {
            await DisplayAlert("Hata", "Yeni sezon eklenemedi", "Tamam");
            return;
        }

        var yeniSezonJson = await ekleResponse.Content.ReadAsStringAsync();
        var sezon = System.Text.Json.JsonSerializer.Deserialize<DiziSezon>(yeniSezonJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (sezon != null)
        {
            _sezonlar.Add(sezon);
            SezonCollectionView.ItemsSource = null;
            SezonCollectionView.ItemsSource = _sezonlar;
        }
    }

    // Sezona tıklanınca
    private void SezonCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is DiziSezon secilenSezon)
        {
            _seciliSezon = secilenSezon;
            BolumEklePanel.IsVisible = true;

            // BindingContext güncelle (isteğe bağlı)
            BolumEklePanel.BindingContext = this;
            OnPropertyChanged(nameof(SelectedSezonLabel));

            // Entry alanlarını temizle
            txtBolumAdi.Text = "";
            txtThumbnailUrl.Text = "";
        }
        else
        {
            _seciliSezon = null;
            BolumEklePanel.IsVisible = false;
        }
    }

    // Bölüm ekle butonuna basıldığında
    private async void BolumEkle_Clicked(object sender, EventArgs e)
    {
        if (_seciliSezon == null)
        {
            await DisplayAlert("Hata", "Lütfen önce bir sezon seçin.", "Tamam");
            return;
        }

        string bolumAdi = txtBolumAdi.Text?.Trim() ?? "";
        string thumbnailUrl = txtThumbnailUrl.Text?.Trim();

        if (string.IsNullOrWhiteSpace(bolumAdi))
        {
            await DisplayAlert("Hata", "Bölüm adı boş olamaz.", "Tamam");
            return;
        }

        using var client = new HttpClient();

        // Yeni bölüm için DTO
        var yeniBolum = new
        {
            SezonId = _seciliSezon.Id,
            BolumNo = (_seciliSezon.Bolumler.Count > 0 ? _seciliSezon.Bolumler.Max(b => b.BolumNo) : 0) + 1,
            Ad = bolumAdi,
            ThumbnailUrl = thumbnailUrl,
            BolumUrl = (string?)null,
            // Tarih sunucu tarafında DateTime.Now atanabilir.
        };

        var response = await client.PostAsJsonAsync("https://localhost:7248/api/bolumler", yeniBolum);
        if (!response.IsSuccessStatusCode)
        {
            await DisplayAlert("Hata", "Bölüm eklenemedi.", "Tamam");
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        var eklenenBolum = System.Text.Json.JsonSerializer.Deserialize<DiziBolum>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (eklenenBolum != null)
        {
            _seciliSezon.Bolumler.Add(eklenenBolum);

            // Sezonların CollectionView'ini yenile (örneğin tekrar ata)
            SezonCollectionView.ItemsSource = null;
            SezonCollectionView.ItemsSource = _sezonlar;

            // Paneli temizle
            txtBolumAdi.Text = "";
            txtThumbnailUrl.Text = "";

            await DisplayAlert("Başarılı", "Yeni bölüm eklendi.", "Tamam");
        }
    }









    public class SezonDto
    {
        public int Id { get; set; }
        public int SezonNo { get; set; }
    }

    public class BolumDto
    {
        public int Id { get; set; }
        public int BolumNo { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
    }

    public class DiziBolumEkleDto
    {
        public int BolumNo { get; set; }
        public string Ad { get; set; } = string.Empty;
        public int SezonId { get; set; }
        public string? BolumUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
    }

    public class DiziEkleModel
    {
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public DateTime YayınTarihi { get; set; }
        public string Yonetmen { get; set; }
        public decimal Puan { get; set; }
        public int KategoriId { get; set; }
        public string? ImageUrl { get; set; }
    }

    
}