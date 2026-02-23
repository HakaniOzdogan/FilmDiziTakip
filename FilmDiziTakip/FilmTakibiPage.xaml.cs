using FilmDiziTakip.Modeller;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FilmDiziTakip;

public partial class FilmTakibiPage : ContentPage
{
    private ObservableCollection<Film> filmler = new();
    private List<Kategori> kategoriler = new();
    private ObservableCollection<YorumFilm> yorumlar = new();
    private int seciliFilmId = 0;
    private int _filmId = 0;

    public FilmTakibiPage()
    {
        InitializeComponent();
        filmListesiView.ItemsSource = filmler;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await KategorileriYukle();
        await SayfaYukle();
    }

    private async Task KategorileriYukle()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("https://localhost:7248/kategori/listele");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var kategoriListesi = JsonSerializer.Deserialize<List<Kategori>>(json);

            if (kategoriListesi != null)
            {
                kategoriler = kategoriListesi;
                kategoriPicker.ItemsSource = kategoriler;
            }
        }
        else
        {
            await DisplayAlert("Hata", "Kategori verisi alýnamadý", "Tamam");
        }
    }

    private async Task FilmListele()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7248/film/listele");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var filmListesi = JsonSerializer.Deserialize<List<Film>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                filmler.Clear();

                if (filmListesi != null)
                {
                    foreach (var film in filmListesi)
                    {
                        var kategori = kategoriler.FirstOrDefault(k => k.Id == film.KategoriId);
                        film.KategoriAd = kategori?.Ad ?? "Bilinmiyor";
                        filmler.Add(film);
                    }
                }
            }
            else
            {
                await DisplayAlert("Hata", "Film listesi alýnamadý", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluþtu: {ex.Message}", "Tamam");
        }
    }
    private async Task SayfaYukle()
    {
        await KategoriListele();
        await FilmListele();
    }

    private async Task KategoriListele()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("https://localhost:7248/kategori/listele");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var kategoriListesi = JsonSerializer.Deserialize<List<Kategori>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (kategoriListesi != null)
            {
                kategoriler = kategoriListesi;
                kategoriPicker.ItemsSource = kategoriler;
            }
        }
    }



    private async void BtnFilmEkle_Clicked(object sender, EventArgs e)
    {
        var seciliKategori = kategoriPicker.SelectedItem as Kategori;

        var yeniFilm = new Film
        {
            Ad = txtAd.Text,
            Aciklama = txtAciklama.Text,
            Yonetmen = txtYonetmen.Text,
            YayýnTarihi = dateYayýnTarihi.Date,
            CikisTarihi = dateCikisTarihi.Date,
            KategoriId = seciliKategori?.Id ?? 0,
            ImageUrl = txtImageUrl.Text,
            FilmUrl = txtVideoUrl.Text
        };

        var json = JsonSerializer.Serialize(yeniFilm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PostAsync("https://localhost:7248/film/ekle", content);


        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Baþarýlý", "Film eklendi", "Tamam");
            Temizle();
            await FilmListele();
        }
        else
        {
            await DisplayAlert("Hata", "Film eklenemedi", "Tamam");
        }
    }

    private void filmListesiView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Film seciliFilm)
        {
            _filmId = seciliFilm.Id;

            txtAd.Text = seciliFilm.Ad;
            txtAciklama.Text = seciliFilm.Aciklama;
            txtYonetmen.Text = seciliFilm.Yonetmen;
            dateYayýnTarihi.Date = seciliFilm.YayýnTarihi;
            dateCikisTarihi.Date = seciliFilm.CikisTarihi;
            kategoriPicker.SelectedItem = kategoriler.FirstOrDefault(k => k.Id == seciliFilm.KategoriId);
            txtImageUrl.Text = seciliFilm.ImageUrl;
            txtVideoUrl.Text = seciliFilm.FilmUrl;

            filmListesiView.SelectedItem = null;
        }
    }


    private async void IzleButton_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && !string.IsNullOrWhiteSpace(txtVideoUrl.Text))
        {
            await Launcher.Default.OpenAsync(txtVideoUrl.Text);
        }
    }



    private async void BtnFilmTemizle_Clicked(object sender, EventArgs e)
    {
        Temizle();
    }

    private void Temizle()
    {
        txtAd.Text = string.Empty;
        txtAciklama.Text = string.Empty;
        txtYonetmen.Text = string.Empty;
        dateYayýnTarihi.Date = DateTime.Today;
        dateCikisTarihi.Date = DateTime.Today;
        kategoriPicker.SelectedItem = null;
        txtImageUrl.Text = string.Empty;
        txtVideoUrl.Text = string.Empty;
        _filmId = 0;
    }
    private async void TekilSilTiklandi(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var secilenFilm = button.CommandParameter as Film;

        if (secilenFilm == null)
            return;

        bool onay = await DisplayAlert("Onay", $"'{secilenFilm.Ad}' filmini silmek istiyor musunuz?", "Evet", "Hayýr");
        if (!onay) return;

        try
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7248/film/sil?id={secilenFilm.Id}");

            if (response.IsSuccessStatusCode)
            {
                filmler.Remove(secilenFilm);
                await DisplayAlert("Baþarýlý", "Film silindi.", "Tamam");
            }
            else
            {
                await DisplayAlert("Hata", "Film silinemedi.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

    private async void BtnFilmGuncelle_Clicked(object sender, EventArgs e)
    {
        if (_filmId == 0)
        {
            await DisplayAlert("Hata", "Güncellenecek bir film seçilmedi.", "Tamam");
            return;
        }

        var seciliKategori = kategoriPicker.SelectedItem as Kategori;

        var guncellenmisFilm = new Film
        {
            Id = _filmId,
            Ad = txtAd.Text,
            Aciklama = txtAciklama.Text,
            Yonetmen = txtYonetmen.Text,
            YayýnTarihi = dateYayýnTarihi.Date,
            CikisTarihi = dateCikisTarihi.Date,
            KategoriId = seciliKategori?.Id ?? 0,
            ImageUrl = txtImageUrl.Text,
            FilmUrl = txtVideoUrl.Text
        };

        var json = JsonSerializer.Serialize(guncellenmisFilm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PutAsync($"https://localhost:7248/film/guncelle/{_filmId}", content);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Baþarýlý", "Film güncellendi.", "Tamam");
            Temizle();
            await FilmListele();
            _filmId = 0; // iþlem sonrasý sýfýrla
        }
        else
        {
            await DisplayAlert("Hata", "Film güncellenemedi.", "Tamam");
        }
    }
    private async void BtnYorumlariGoster_Clicked(object sender, EventArgs e)
    {
        if (_filmId == 0)
        {
            await DisplayAlert("Uyarý", "Lütfen önce bir film seçin.", "Tamam");
            return;
        }

        yorumBaslikFrame.IsVisible = true;
        yorumListView.IsVisible = true;
        await YorumlariYukle(_filmId);
    }


    private async Task YorumlariYukle(int filmId)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://localhost:7248/yorum/film?filmId={filmId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var gelenYorumlar = JsonSerializer.Deserialize<List<YorumFilm>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                yorumlar.Clear();
                foreach (var y in gelenYorumlar)
                    yorumlar.Add(y);

                yorumListView.ItemsSource = yorumlar;
            }
            else
            {
                await DisplayAlert("Hata", "Yorumlar getirilemedi.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }
    private async void YorumSil_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton btn && btn.CommandParameter is YorumFilm yorum)
        {
            bool onay = await DisplayAlert("Onay", "Yorumu silmek istiyor musunuz?", "Evet", "Hayýr");
            if (!onay) return;

            try
            {
                using var client = new HttpClient();
                var response = await client.DeleteAsync($"https://localhost:7248/yorum/film?id={yorum.Id}");

                if (response.IsSuccessStatusCode)
                {
                    yorumlar.Remove(yorum);
                }
                else
                {
                    await DisplayAlert("Hata", "Silme baþarýsýz.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
    }
}