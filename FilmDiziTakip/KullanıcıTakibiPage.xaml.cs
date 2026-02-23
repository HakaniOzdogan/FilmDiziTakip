using FilmDiziTakip.Modeller;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FilmDiziTakip;

public partial class KullaniciTakibiPage : ContentPage
{
    private ObservableCollection<Kullanici> kullanicilar = new();
    private int _kullaniciId = 0;

    public KullaniciTakibiPage()
    {
        InitializeComponent();
        kullaniciListesiView.ItemsSource = kullanicilar;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await KullanicilariListele();
    }

    private async Task KullanicilariListele()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7248/kullanici/listele");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var liste = JsonSerializer.Deserialize<List<Kullanici>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                kullanicilar.Clear();

                if (liste != null)
                {
                    foreach (var kullanici in liste)
                        kullanicilar.Add(kullanici);
                }
            }
            else
            {
                await DisplayAlert("Hata", "Kullanýcýlar alýnamadý", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

    private async void BtnKullaniciEkle_Clicked(object sender, EventArgs e)
    {
        var yeni = new Kullanici
        {
            KullaniciAdi = txtKullaniciAdi.Text,
            Sifre = txtSifre.Text,
            OlusturulmaTarihi = dateOlusturulma.Date
        };

        var json = JsonSerializer.Serialize(yeni);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PostAsync("https://localhost:7248/kullanici/ekle", content);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Baþarýlý", "Kullanýcý eklendi", "Tamam");
            Temizle();
            await KullanicilariListele();
        }
        else
        {
            await DisplayAlert("Hata", "Kullanýcý eklenemedi", "Tamam");
        }
    }

    private void kullaniciListesiView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Kullanici secili)
        {
            _kullaniciId = secili.Id;
            txtKullaniciAdi.Text = secili.KullaniciAdi;
            txtSifre.Text = secili.Sifre;
            dateOlusturulma.Date = secili.OlusturulmaTarihi;

            kullaniciListesiView.SelectedItem = null;
        }
    }

    private async void TekilSilTiklandi(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var secilen = button.CommandParameter as Kullanici;

        if (secilen == null)
            return;

        bool onay = await DisplayAlert("Onay", $"'{secilen.KullaniciAdi}' adlý kullanýcýyý silmek istiyor musunuz?", "Evet", "Hayýr");
        if (!onay) return;

        try
        {
            using var client = new HttpClient();
            var response = await client.DeleteAsync($"https://localhost:7248/kullanici/sil/{secilen.Id}");

            if (response.IsSuccessStatusCode)
            {
                kullanicilar.Remove(secilen);
                await DisplayAlert("Baþarýlý", "Kullanýcý silindi.", "Tamam");
            }
            else
            {
                await DisplayAlert("Hata", "Kullanýcý silinemedi.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }

     

    private async void BtnKullaniciGuncelle_Clicked(object sender, EventArgs e)
    {
        if (_kullaniciId == 0)
        {
            await DisplayAlert("Hata", "Güncellenecek bir kullanýcý seçilmedi.", "Tamam");
            return;
        }

        var guncellenmisKullanici = new Kullanici
        {
            Id = _kullaniciId,
            KullaniciAdi = txtKullaniciAdi.Text,
            Sifre = txtSifre.Text,
            OlusturulmaTarihi = dateOlusturulma.Date
        };

        var json = JsonSerializer.Serialize(guncellenmisKullanici);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PutAsync($"https://localhost:7248/kullanici/guncelle/{_kullaniciId}", content);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Baþarýlý", "Kullanýcý güncellendi.", "Tamam");
            Temizle();
            await KullanicilariListele(); // Listeyi yenile
            _kullaniciId = 0; // iþlem sonrasý sýfýrla
        }
        else
        {
            await DisplayAlert("Hata", "Kullanýcý güncellenemedi.", "Tamam");
        }
    }
    private bool sifreGorunur = false;

    private void btnSifreGoster_Clicked(object sender, EventArgs e)
    {
        sifreGorunur = !sifreGorunur;
        txtSifre.IsPassword = !sifreGorunur;

        btnSifreGoster.Source = sifreGorunur ? "eye_off.png" : "eye.png";
    }

    private async void BtnKullaniciTemizle_Clicked(object obj, EventArgs e)
    {
        Temizle();
    }


    private void Temizle()
    {
        txtKullaniciAdi.Text = string.Empty;
        txtSifre.Text = string.Empty;
        dateOlusturulma.Date = DateTime.Today;
        _kullaniciId = 0;
    }
}
