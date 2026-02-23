using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;
using FilmDiziTakip.Modeller;

namespace FilmDiziTakip
{
    public partial class FilmDiziAramaPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        public int? _kullaniciId;

        public FilmDiziAramaPage(int? _kullaniciIdsi)
        {
            InitializeComponent();
            _kullaniciId = _kullaniciIdsi;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7248/") // Bunu kendi API adresinle deðiþtir
            };
        }
        private async void GeriDon(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KullaniciMainPage(_kullaniciId));
        }


        public async Task<List<Arama>> FilmDiziAraAsync(string ad)
        {
            if (string.IsNullOrWhiteSpace(ad))
                return new List<Arama>();

            try
            {
                return await _httpClient.GetFromJsonAsync<List<Arama>>($"arama?ad={Uri.EscapeDataString(ad)}")
                       ?? new List<Arama>();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Arama yapýlamadý: " + ex.Message, "Tamam");
                return new List<Arama>();
            }
        }


        private async void AraBtn_Clicked(object sender, EventArgs e)
        {
            var aramaKelimesi = aramaEntry.Text;
            var sonuclar = await FilmDiziAraAsync(aramaKelimesi);

            if (sonuclar == null || sonuclar.Count == 0)
            {
                await DisplayAlert("Sonuç Yok", "Aradýðýnýz isimde bir film veya dizi bulunamadý.", "Tamam");
            }

            filmlerdizilerListView.ItemsSource = sonuclar;
        }


        private async void FilmDiziSecildi(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Arama secilen)
            {
                if (secilen.Tur == TurEnum.Film)
                {
                    await Navigation.PushAsync(new FilmDetayPage(secilen.Id,_kullaniciId));
                }
                else if (secilen.Tur == TurEnum.Dizi)
                {
                    await Navigation.PushAsync(new DiziDetayPage(secilen.Id,_kullaniciId));
                }

                ((CollectionView)sender).SelectedItem = null;
            }
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
