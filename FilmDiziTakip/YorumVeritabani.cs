using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FilmDiziTakip
{
    public static class YorumVeritabani
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _baseApiUrl = "https://localhost:7248/yorum/ekle";

        public static async Task<bool> YorumEkleAsync(int filmId, int kullaniciId, string yorumMetni, int puan)
        {
            var yeniYorum = new
            {
                FilmId = filmId,
                KullaniciId = kullaniciId,
                YorumMetni = yorumMetni,
                Puan = puan
            };

            string json = JsonSerializer.Serialize(yeniYorum);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/ekle", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[YorumEkleAsync] Hata: {ex.Message}");
                return false;
            }
        }

        public static async Task<List<string>> YorumlariGetirAsync(int filmId)
        {
            var yorumlar = new List<string>();

            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/film/{filmId}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[YorumlariGetirAsync] API hatası: {response.StatusCode}");
                    return yorumlar;
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var gelenYorumlar = JsonSerializer.Deserialize<List<YorumDTO>>(json, options);

                if (gelenYorumlar != null)
                {
                    foreach (var yorum in gelenYorumlar)
                    {
                        yorumlar.Add($"{yorum.Puan}★ - {yorum.YorumMetni} ({yorum.Tarih:g})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[YorumlariGetirAsync] Hata: {ex.Message}");
            }

            return yorumlar;
        }
    }

    public class YorumDTO
    {
        public string YorumMetni { get; set; }
        public int Puan { get; set; }
        public DateTime Tarih { get; set; }
    }
}
