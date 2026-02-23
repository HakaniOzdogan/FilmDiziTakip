using FilmDiziTakip.Modeller;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.ObjectModel;

using System.Net.Http;
using System.Net.Http.Json; 


namespace FilmDiziTakip;

public partial class IzlemeGecmisiPage : ContentPage
{
    private readonly int? _kullaniciId;

    public ObservableCollection<FilmDizi> Izlenenler { get; set; } = new();
    public ObservableCollection<IzlemeDetayModel> IzlenenDiziler { get; set; } = new();
    public IzlemeGecmisiPage(int? kullaniciId)
    {
        InitializeComponent();
        _kullaniciId = kullaniciId;
        BindingContext = this;
    }

  

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await IzlemeGecmisiniYukle();
        await IzlemeGecmisiniYukleDizi();
    }

    private async Task IzlemeGecmisiniYukle()
    {
        try
        {
            HttpClient client = new();
            string url = $"https://localhost:7248/izleme-durumu/filmler?kullaniciId={_kullaniciId}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Hata", "Ýzleme geçmiþi alýnamadý.", "Tamam");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var gecmisListesi = JsonSerializer.Deserialize<List<FilmDizi>>(json, options);

            Izlenenler.Clear();
            if (gecmisListesi != null)
            {
                foreach (var item in gecmisListesi)
                    Izlenenler.Add(item);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluþtu: {ex.Message}", "Tamam");
        }
    }

    private async void SecimYapildi(object sender, SelectionChangedEventArgs e)
    {
        var secilen = e.CurrentSelection.FirstOrDefault() as FilmDizi;
        if (secilen == null) return;

        if (secilen.Tur==1)
            await Navigation.PushAsync(new FilmDetayPage(secilen.Id, _kullaniciId));
 
        else
           await Navigation.PushAsync(new DiziDetayPage(secilen.Id, _kullaniciId));
          

        ((CollectionView)sender).SelectedItem = null;
    }

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new KullaniciMainPage(_kullaniciId));

    }


    private async void CikisYap(object sender, EventArgs e)
    {
        // Kullanýcýya çýkýþ yaptýðýný bildir
        await DisplayAlert("Çýkýþ", "Çýkýþ yaptýnýz. Lütfen tekrar giriþ yapmak için bilgilerinizi giriniz.", "Tamam");

        // MainPage'e yönlendirme (örneðin giriþ sayfasý)
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }


    private async void TekilSilTiklandi(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;

        if (button.CommandParameter is FilmDizi secilenFilm)
        {
            bool onay = await DisplayAlert("Onay", $"'{secilenFilm.Ad}' izleme geçmiþinden silinsin mi?", "Evet", "Hayýr");
            if (!onay) return;

            try
            {
                HttpClient client = new();
                string url = $"https://localhost:7248/izleme-durumu/sil?kullaniciId={_kullaniciId}&filmId={secilenFilm.Id}";
                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    Izlenenler.Remove(secilenFilm);
                }
                else
                {
                    await DisplayAlert("Hata", "Film geçmiþten silinemedi.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", ex.Message, "Tamam");
            }
        }
        
        else
        {
            await DisplayAlert("Hata", "Geçersiz öðe seçildi.", "Tamam");
        }
    }

    private async Task IzlemeGecmisiniYukleDizi()
    {
        try
        {
            using HttpClient client = new();
            string url = $"https://localhost:7248/izleme-durumu_dizi/liste?kullaniciId={_kullaniciId}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Hata", "Dizi izleme geçmiþi alýnamadý.", "Tamam");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var diziListesi = JsonSerializer.Deserialize<List<IzlemeDetayModel>>(json, options);

            IzlenenDiziler.Clear();
            if (diziListesi != null)
            {
                foreach (var dizi in diziListesi)
                    IzlenenDiziler.Add(dizi);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluþtu: {ex.Message}", "Tamam");
        }
    }

    private async void TekilSilTiklandiDizi(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;
        var secilen = (IzlemeDetayModel)button.CommandParameter; // IzlemeDetayModel kullanýyoruz

        bool onay = await DisplayAlert("Onay", $"{secilen.DiziAdi} {secilen.SezonNo}. Sezon {secilen.BolumNo}. Bölüm izleme geçmiþinden silinsin mi?", "Evet", "Hayýr");
        if (!onay) return;

        try
        {
            HttpClient client = new();
            string url = $"https://localhost:7248/izleme-durumu-dizi/sil?kullaniciId={App.GirisYapanKullaniciId}&diziBolumId={secilen.BolumId}";
            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                IzlenenDiziler.Remove(secilen);
            }
            else
            {
                var hataMesaji = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Hata", $"Dizi geçmiþten silinemedi: {hataMesaji}", "Tamam");

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }



    private async void TumGecmisiSil(object sender, EventArgs e)
    {
        // Eðer hem film hem dizi listesi zaten boþsa
        if (!Izlenenler.Any() && !IzlenenDiziler.Any())
        {
            await DisplayAlert("Bilgi", "Geçmiþ zaten temiz.", "Tamam");
            return;
        }

        bool onay = await DisplayAlert("Onay", "Tüm izleme geçmiþiniz silinsin mi?", "Evet", "Hayýr");
        if (!onay) return;

        try
        {
            HttpClient client = new();

            bool filmSilindi = true;
            bool diziSilindi = true;

            // Film geçmiþi varsa sil
            if (Izlenenler.Any())
            {
                string urlFilm = $"https://localhost:7248/izleme-durumu/tumunu-sil?kullaniciId={_kullaniciId}";
                var responseFilm = await client.DeleteAsync(urlFilm);
                filmSilindi = responseFilm.IsSuccessStatusCode;
            }

            // Dizi geçmiþi varsa sil
            if (IzlenenDiziler.Any())
            {
                string urlDizi = $"https://localhost:7248/izleme-durumu-dizi/tumunu-sil?kullaniciId={_kullaniciId}";
                var responseDizi = await client.DeleteAsync(urlDizi);
                diziSilindi = responseDizi.IsSuccessStatusCode;
            }

            if (filmSilindi && diziSilindi)
            {
                Izlenenler.Clear();
                IzlenenDiziler.Clear();
                await DisplayAlert("Baþarýlý", "Ýzleme geçmiþiniz temizlendi.", "Tamam");
            }
            else
            {
                await DisplayAlert("Uyarý", "Bazý geçmiþler silinemedi.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", ex.Message, "Tamam");
        }
    }




    private async void IzlemeGecmisiListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is IzlemeDetayModel secilen)
        {
            // 1- API'dan seçilen dizinin tüm bölümlerini çek
            var bolumler = await BolumleriGetir(secilen.DiziId);

            // 2- Týklanan bölümün indexini bul
            int startIndex = bolumler.FindIndex(b => b.Id == secilen.BolumId);

            // 3- BolumDetayPage sayfasýný aç, bölümler listesi + index + diðer bilgilerle
            await Navigation.PushAsync(new BolumDetayPage(bolumler, startIndex, secilen.DiziAdi, secilen.DiziId));

            // Seçimi temizle
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async Task<List<DiziBolum>> BolumleriGetir(int diziId)
    {
        using HttpClient client = new();
        string url = $"https://localhost:7248/dizi/{diziId}/bolumler";  // API'da böyle bir endpoint olmalý
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            await DisplayAlert("Hata", "Dizi bölümleri alýnamadý.", "Tamam");
            return new List<DiziBolum>();
        }

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var bolumler = JsonSerializer.Deserialize<List<DiziBolum>>(json, options);
        return bolumler ?? new List<DiziBolum>();
    }

}
