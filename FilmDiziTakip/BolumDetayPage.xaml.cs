using CommunityToolkit.Maui.Views;
using System.Text.RegularExpressions;
using System.Text;
using FilmDiziTakip.Modeller;
using CommunityToolkit.Maui.Core.Primitives;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FilmDiziTakip
{
    public partial class BolumDetayPage: ContentPage
    {
        private List<DiziBolum> _bolumler;
        private int _currentIndex;
        private string _diziAdi;
        private int _diziId;
        private int _diziBolumId;
        private int? _kullaniciId;
        private int _secilenPuan = 0;
        private readonly HttpClient _httpClient = new();
        public BolumDetayPage(List<DiziBolum> bolumler, int startIndex,string diziAdi,int diziId)
        {
            InitializeComponent();
            
            _bolumler = bolumler ?? new List<DiziBolum>();
            _currentIndex = (startIndex >=0  && startIndex < _bolumler.Count) ? startIndex : 0;
            _diziAdi = diziAdi; // 
            _diziId = diziId;
            _diziBolumId = _bolumler[_currentIndex].Id; ;
            _kullaniciId = App.GirisYapanKullaniciId;

            InitYildizEtkinlikleri();
            
            YukleBolumAsync(_bolumler[_currentIndex]);
            UpdateButtonStates();
             YukleBolumYorumlariAsync();
        }



        private async Task YukleBolumAsync(DiziBolum bolum)
        {

            int sezonNo =0;

            var response = await _httpClient.GetAsync($"https://localhost:7248/bolumler/{_diziBolumId}/sezon");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var sezonBilgi = System.Text.Json.JsonSerializer.Deserialize<BolumSezonDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (sezonBilgi != null)
                {
                    sezonNo = sezonBilgi.SezonNo;
                }
            }






            _diziBolumId = bolum.Id;
            BolumAdiLabel.Text = $"{sezonNo}. Sezon{bolum.BolumNo}. Bölüm - {bolum.Ad}";
            DiziAdiLabel.Text = $"{_diziAdi}";
            // 📂 4. Önce BolumUrl kullan, yoksa yerel dosya kontrolü yap
            string temizDiziAdi = Regex.Replace(bolum.Ad, $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", "_");
            string dosyaAdi = $"{_diziAdi}_Sezon{sezonNo}_Bolum{bolum.BolumNo}.mp4";
            string videoPath = Path.Combine(@"C:\Users\alpak\OneDrive\Masaüstü\proje\FilmDiziTakip\Resources\DiziBolumler", dosyaAdi);

            // Önce online URL varsa kullan
            if (!string.IsNullOrWhiteSpace(bolum.BolumUrl))
            {
                BolumOynatici.Source = MediaSource.FromUri(bolum.BolumUrl);
            }
            // Yerel dosya varsa onu oynat
            else if (File.Exists(videoPath))
            {
                BolumOynatici.Source = MediaSource.FromFile(videoPath);
            }
            else
            {
                await DisplayAlert("Hata", $"Video bulunamadı: {videoPath}", "Tamam");
            }
        }

        private void UpdateButtonStates()
        {
            btnOnceki.IsEnabled = _currentIndex > 0;
            btnSonraki.IsEnabled = _currentIndex < _bolumler.Count - 1;
        }

        private async void OncekiButtonTiklandi(object sender, EventArgs e)
        {

            double izlenenSure = BolumOynatici.Position.TotalSeconds;
            await GecmiseEkleAsync(_diziBolumId, App.GirisYapanKullaniciId, izlenenSure);

            if (_currentIndex > 0)
            {
                _currentIndex--;
                await YukleBolumAsync(_bolumler[_currentIndex]);

                var sure = await GetKaldigiSureAsync(App.GirisYapanKullaniciId, _diziBolumId);

                if (sure > 0)
                {
                    bool devam = await DisplayAlert("Devam Et", $"Videoyu {Math.Round(sure)}. saniyeden devam ettirmek ister misiniz?", "Evet", "Hayır");
                    if (devam)
                        BolumOynatici.SeekTo(TimeSpan.FromSeconds(sure));
                }

                BolumOynatici.Play();

                UpdateButtonStates();
                await YukleBolumYorumlariAsync();
                await OrtalamaPuaniGuncelle();
            }
        }

        private async void SıradakiButtonTiklandi(object sender, EventArgs e)
        {

            double izlenenSure = BolumOynatici.Position.TotalSeconds;
            await GecmiseEkleAsync(_diziBolumId, App.GirisYapanKullaniciId, izlenenSure);

            if (_currentIndex < _bolumler.Count - 1)
            {
                _currentIndex++;
                await YukleBolumAsync(_bolumler[_currentIndex]);
                UpdateButtonStates();

                var sure = await GetKaldigiSureAsync(App.GirisYapanKullaniciId, _diziBolumId);

                if (sure > 0)
                {
                    bool devam = await DisplayAlert("Devam Et", $"Videoyu {Math.Round(sure)}. saniyeden devam ettirmek ister misiniz?", "Evet", "Hayır");
                    if (devam)
                        BolumOynatici.SeekTo(TimeSpan.FromSeconds(sure));
                }



                BolumOynatici.Play();

                await YukleBolumYorumlariAsync();
                await OrtalamaPuaniGuncelle();
            }
        }

        private async void GeriDon(object sender, EventArgs e)
        {
            double izlenenSure = BolumOynatici.Position.TotalSeconds;

            // 🔄 İzlenme süresini kaydet
            await GecmiseEkleAsync(_diziBolumId, App.GirisYapanKullaniciId, izlenenSure);

            await Navigation.PushAsync(new DiziDetayPage(_diziId,App.GirisYapanKullaniciId));
        }

        private async void CikisYap(object sender, EventArgs e)
        {
            
            await DisplayAlert("Çıkış", "Çıkış yaptınız. Lütfen tekrar giriş yapmak için bilgilerinizi giriniz.", "Tamam");

           
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }


        private async Task GecmiseEkleAsync(int bolumId, int? kullaniciId, double izlenenSure)
        {
            if (kullaniciId == null || bolumId == 0)
                return;

            var dto = new
            {
                DiziBolumId = bolumId,
                KullaniciId = kullaniciId,
                IzlenmeSuresi = izlenenSure
            };

            var json = System.Text.Json.JsonSerializer.Serialize(dto);


            // Encoding'i açıkça belirt
            var encoding = new UTF8Encoding(false);
            var content = new StringContent(json, encoding, "application/json");

            HttpClient client = new();
            await client.PostAsync("https://localhost:7248/izleme-durumu_dizi/ekle", content);
        }


        private void InitYildizEtkinlikleri()
        {
            foreach (var view in YildizPanel.Children.OfType<Label>())
            {
                view.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => YildizTiklandi(view))
                });
            }
        }

        private async Task YukleBolumYorumlariAsync()
        {
            try
            {
                var yorumlar = await YorumlariGetirAsync(_diziBolumId);
                YorumlarList.ItemsSource = yorumlar;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Yorumlar yüklenirken bir hata oluştu: " + ex.Message, "Tamam");
            }
        }

        private void GuncelleYildizGorunumu(int puan)
        {
            foreach (var label in YildizPanel.Children.OfType<Label>())
            {
                if (int.TryParse(label.ClassId, out int index))
                {
                    label.TextColor = index <= puan ? Colors.Gold : Colors.Gray;
                    label.Text = "★";
                }
            }
        }

        private async void YildizTiklandi(Label tiklanan)
        {
            try
            {
                if (!int.TryParse(tiklanan.ClassId, out int puan))
                    return;

                _secilenPuan = puan;
                GuncelleYildizGorunumu(puan);

                var yeniYorum = new DiziBolumYorumDto
                {
                    DiziBolumId = _diziBolumId,
                    KullaniciId = _kullaniciId,
                    Puan = puan,
                    Yorum = null,
                    YorumTarihi = DateTime.Now
                };

                var json = JsonConvert.SerializeObject(yeniYorum);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://localhost:7248/bolumyorumlar", content);

                if (response.IsSuccessStatusCode)
                {
                    BildirimLabel.Text = "Puan başarıyla gönderildi.";
                    BildirimLabel.IsVisible = true;
                    await Task.Delay(2000);
                    BildirimLabel.IsVisible = false;

                    // Yorumları ve puan ortalamasını güncelle
                    await YukleBolumYorumlariAsync();
                    await OrtalamaPuaniGuncelle();
                }
                else
                {
                    
                    
                    await DisplayAlert("Hata", "Puan gönderilemedi.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Bir hata oluştu:\n{ex.Message}", "Tamam");
            }
        }

        private async Task OrtalamaPuaniGuncelle()
        {
            var yorumlar = await YorumlariGetirAsync(_bolumler[_currentIndex].Id);

            if (yorumlar.Count == 0)
            {
                foreach (var star in OrtalamaPuanPanel.Children.OfType<Label>())
                {
                    star.Text = "☆";
                    star.TextColor = Colors.Gray;
                }
                return;
            }

            double ortalama = yorumlar.Average(y => y.Puan);
            int doluYildiz = (int)Math.Round(ortalama);

            int index = 0;
            foreach (var star in OrtalamaPuanPanel.Children.OfType<Label>())
            {
                if (index < doluYildiz)
                {
                    star.Text = "★";
                    star.TextColor = Colors.Goldenrod;
                }
                else
                {
                    star.Text = "☆";
                    star.TextColor = Colors.Gray;
                }
                index++;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 🔄 Kullanıcının bıraktığı süreyi getir
            var sure = await GetKaldigiSureAsync(App.GirisYapanKullaniciId, _diziBolumId);

            if (sure > 0)
            {
                int dakika = (int)(sure / 60);
                int saniye = (int)(sure % 60);
                if (dakika > 0) 
                { 
                    bool devam = await DisplayAlert("Devam Et", $"Videoyu {dakika}. dakika {saniye}. saniyeden devam ettirmek ister misiniz?", "Evet", "Hayır");
                    if (devam)
                        BolumOynatici.SeekTo(TimeSpan.FromSeconds(sure));
                }
                else
                {
                    bool devam = await DisplayAlert("Devam Et", $"Videoyu {saniye}. saniyeden devam ettirmek ister misiniz?", "Evet", "Hayır");
                    if (devam)
                        BolumOynatici.SeekTo(TimeSpan.FromSeconds(sure));

                }

                
            }

            BolumOynatici.Play();

            OrtalamaPuaniGuncelle(); // Ortalama puanı her sayfa gösterildiğinde güncelle
        }

        private async Task<double> GetKaldigiSureAsync(int? kullaniciId, int bolumId)
        {
            try
            {
                HttpClient client = new();
                string url = $"https://localhost:7248/izleme-durumu_dizi/sure?kullaniciId={kullaniciId}&bolumId={bolumId}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return 0;

                var json = await response.Content.ReadAsStringAsync();
                var sure = System.Text.Json.JsonSerializer.Deserialize<double>(json);
                return sure;
            }
            catch
            {
                return 0;
            }
        }


        private async void GonderYorumClicked(object sender, EventArgs e)
        {
            var metin = KullaniciYorumEditor.Text?.Trim();
            if (string.IsNullOrWhiteSpace(metin))
            {
                await DisplayAlert("Uyarı", "Lütfen yorum metni giriniz.", "Tamam");
                return;
            }

           var yorumDto = new DiziBolumYorumDto
            {
                DiziBolumId = _diziBolumId,
                KullaniciId = _kullaniciId,
                Puan = _secilenPuan,
                Yorum = metin,
                YorumTarihi = DateTime.Now
           };




            var response = await _httpClient.PostAsync("https://localhost:7248/bolumyorumlar",
                new StringContent(JsonConvert.SerializeObject(yorumDto), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                KullaniciYorumEditor.Text = string.Empty;
                await DisplayAlert("Başarılı", "Yorum gönderildi.", "Tamam");
                await YukleBolumYorumlariAsync();
                
                await OrtalamaPuaniGuncelle();
            }
            else
            {
                
                await DisplayAlert("Hata", "Puan gönderilemedi.", "Tamam");
            }
        }

        private async Task<List<YorumDto>> YorumlariGetirAsync(int bolumId)
        {
            var yorumJson = await _httpClient.GetStringAsync($"https://localhost:7248/bolumyorumlar/{bolumId}");
            var yorumlar = JsonConvert.DeserializeObject<List<YorumDto>>(yorumJson) ?? new();

            return yorumlar;
        }

        public class DiziBolumYorumDto
        {
            public int? KullaniciId { get; set; }
            public int DiziBolumId { get; set; }
            public int Puan { get; set; }
            public string? Yorum { get; set; }
            public DateTime YorumTarihi { get; set; }
        }

        public class YorumDto
        {
            public string KullaniciAdi { get; set; }
            public string? YorumMetni { get; set; }
            public int Puan { get; set; }
            public DateTime YorumTarihi { get; set; }

            public string Tarih => YorumTarihi.ToString("dd.MM.yyyy HH:mm");
        }

        public class BolumSezonDto
        {
            public int SezonNo { get; set; }
        }


    }
}
