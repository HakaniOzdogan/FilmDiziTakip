using System.Text.Json;
using System.Text;

namespace FilmDiziTakip;

public partial class UyeOlPage : ContentPage
{
	public UyeOlPage()
	{
		InitializeComponent();

      
    }

    private async void GeriDon(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());

    }
    private async void btnUyeOl_Clicked(object sender, EventArgs e)
    {
        string kullaniciAdi = entryKullaniciAdi.Text;
        string sifre = entrySifre.Text;

        if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
        {
            await DisplayAlert("Hata", "Kullanýcý adý ve þifre alanlarý boþ olamaz.", "Tamam");
            return;
        }

        // API adresini belirleyelim
        string apiUrl = "https://localhost:7248/kullanici/ekle";  

        var yeniKullanici = new
        {
            KullaniciAdi = kullaniciAdi,
            Sifre = sifre
        };

        string json = JsonSerializer.Serialize(yeniKullanici);

        try
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Baþarýlý", "Kayýt iþlemi baþarýlý!", "Tamam");
                    await Navigation.PushAsync(new MainPage());  
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Hata", $"Kayýt yapýlamadý: {error}", "Tamam");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluþtu: {ex.Message}", "Tamam");
        }
    }
}