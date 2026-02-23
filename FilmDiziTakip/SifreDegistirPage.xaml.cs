using System.Net.Http.Json;

namespace FilmDiziTakip;

public partial class SifreDegistirPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();

    public SifreDegistirPage()
    {
        InitializeComponent();
        _httpClient.BaseAddress = new Uri("https://localhost:7248"); // kendi API adresinle deðiþtir
    }

    private async void SifreDegistir_Clicked(object sender, EventArgs e)
    {
        lblDurum.Text = "";

        if (string.IsNullOrWhiteSpace(txtEskiSifre.Text) ||
            string.IsNullOrWhiteSpace(txtYeniSifre.Text) ||
            string.IsNullOrWhiteSpace(txtYeniSifreTekrar.Text))
        {
            lblDurum.Text = "Lütfen tüm alanlarý doldurun.";
            return;
        }

        if (txtYeniSifre.Text != txtYeniSifreTekrar.Text)
        {
            lblDurum.Text = "Yeni þifreler uyuþmuyor.";
            return;
        }

        var dto = new
        {
            KullaniciId = App.GirisYapanKullaniciId,
            EskiSifre = txtEskiSifre.Text,
            YeniSifre = txtYeniSifre.Text
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/kullanicilar/sifredegistir", dto);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Baþarýlý", "Þifreniz baþarýyla güncellendi. Lütfen tekrar giriþ yapýnýz.", "Tamam");

                // Oturumu sýfýrla
                App.GirisYapanKullaniciId = 0;
               

                // Navigation stack’i tamamen temizle ve giriþ sayfasýný aç
                Application.Current.MainPage = new NavigationPage(new MainPage());

            }
            else
            {
                var hata = await response.Content.ReadAsStringAsync();
                lblDurum.Text = hata;
            }
        }
        catch (Exception ex)
        {
            lblDurum.Text = "Bir hata oluþtu: " + ex.Message;
        }
    }
}