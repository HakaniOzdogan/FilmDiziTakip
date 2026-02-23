using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using FilmDiziTakip.Modeller;

namespace FilmDiziTakip
{
    public partial class KategoriDetayPage : ContentPage
    {
        private readonly Kategori _kategori;
        public int? _kullaniciId;

        public KategoriDetayPage(Kategori kategori, int? kullaniciId)
        {
            InitializeComponent();
            _kategori = kategori;
            _kullaniciId = kullaniciId;
            kategoriAdiLabel.Text = $"Seçilen Kategori: {_kategori.Ad}";
            Console.WriteLine($"[KategoriDetayPage] Ad: {_kategori.Ad}, Id: {_kategori.Id}");
            _kullaniciId = kullaniciId;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await IcerikleriYukle();
        }

        private async Task IcerikleriYukle()
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = $"https://localhost:7248/arama/kategoriye-gore?kategoriAd={Uri.EscapeDataString(_kategori.Ad)}";

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Hata", "Ýçerikler yüklenemedi.", "Tamam");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("[Gelen JSON]: " + json);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var icerikler = JsonSerializer.Deserialize<List<Arama>>(json, options);

                if (icerikler == null || icerikler.Count == 0)
                {
                    await DisplayAlert("Uyarý", "Ýçerik listesi boþ.", "Tamam");
                    return;
                }

                // CollectionView veya ListView'e ata
                filmListGor.ItemsSource = icerikler;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Ýçerikler alýnamadý: {ex.Message}", "Tamam");
            }
        }

        private async void IcerikSecildi(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Arama secilenIcerik)
            {
                // Türüne göre detay sayfasýna yönlendir
                if (secilenIcerik.Tur == TurEnum.Film)
                {
                    await Navigation.PushAsync(new FilmDetayPage(secilenIcerik.Id, _kullaniciId));
                }
                else if (secilenIcerik.Tur == TurEnum.Dizi)
                {
                    await Navigation.PushAsync(new DiziDetayPage(secilenIcerik.Id,_kullaniciId));
                }

                // Seçimi sýfýrla
                ((CollectionView)sender).SelectedItem = null;
            }
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

    }

}