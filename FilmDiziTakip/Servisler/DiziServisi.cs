using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using Newtonsoft.Json;
using FilmDiziTakip.Modeller;
namespace FilmDiziTakip.Servisler
{
    public class DiziServisi : IDiziService
    {
        private readonly HttpClient _httpClient;

        public DiziServisi()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7248/") // kendi API adresin
            };
        }

        public async Task<List<Dizi>> TumDizileriGetirAsync()
        {
            var response = await _httpClient.GetAsync("dizi/Listele");

            if (response.IsSuccessStatusCode)
            {
              
                var json = await response.Content.ReadAsStringAsync();
                var diziler = JsonConvert.DeserializeObject<List<Dizi>>(json);
                return diziler ?? new List<Dizi>();
            }

            return new List<Dizi>();
        }

        public async Task<Dizi> DiziGetirAsync(int id)
        {
            var response = await _httpClient.GetAsync($"dizi/bul/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dizi = JsonConvert.DeserializeObject<Dizi>(json);
                return dizi!;
            }

            return null!;
        }
    }
}

