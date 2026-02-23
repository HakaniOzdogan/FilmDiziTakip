using System.Net.Http.Json;
namespace FilmDiziTakip;
using FilmDiziTakip.Modeller;
public class KategoriServisi
{
    private readonly HttpClient _httpClient;

    public KategoriServisi()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:7248/") 
        };
    }

    // Kategori Ekleme
    public async Task<bool> AddKategoriAsync(KategoriEkle kategori)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/kategori", kategori);  // API'ye kategori ekle

            // API'den gelen cevaba göre işlem yapma
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                // Hata durumunda işlem yapılabilir
                return false;
            }
        }
        catch (Exception ex)
        {
            // Hata yönetimi
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}
