using System.Text.Json;
using System.Text;
using FilmDiziTakip.Modeller;

namespace FilmDiziTakip
{
    public partial class MainPage : ContentPage
    {

        private int? KullaniciId;
        private string KullaniciAd;

        public MainPage()
        {
            InitializeComponent();
        }

        

       

        private async void KayitBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UyeOlPage());

        }


        private async Task<int?> KullaniciIdGetir(string kullaniciAdi)
        {
            string apiUrl = $"https://localhost:7248/kullanici/bul?kullaniciAdi={kullaniciAdi}";

            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    using var jsonDoc = JsonDocument.Parse(content);
                    var root = jsonDoc.RootElement;

                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        var firstItem = root[0];
                        if (firstItem.TryGetProperty("id", out JsonElement idElement) && idElement.TryGetInt32(out int id))
                        {
                            Console.WriteLine(id);
                            return id;
                        }
                    }

                    return null; // Array boş veya id yok
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Kullanıcı bulunamadı.", "Tamam");
                    return null;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "API hatası.", "Tamam");
                    return null;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"İstek başarısız: {ex.Message}", "Tamam");
                return null;
            }
        }


        private bool sifreGosteriliyor = false;

        // Göz butonunun mevcut durumunu takip eder (true = şifre gösteriliyor)
        private void chkSifreGoster_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            bool sifreGoster = e.Value;

            // Entry'deki şifreyi göster/gizle
            entrySifre.IsPassword = !sifreGoster;

        }

        private async void btnGiris_Clicked(object sender, EventArgs e)
        {
            string kullaniciAdi = entryKullaniciAdi.Text;
            KullaniciAd = kullaniciAdi;
            string sifre = entrySifre.Text;

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                await DisplayAlert("Hata", "Lütfen kullanıcı adı ve şifre giriniz.", "Tamam");
                return;
            }

            // Hangi butona basıldığını öğren
            var button = sender as Button;
            string girisTipi = button?.AutomationId; // "kullanici" ya da "yonetici"

            string apiUrl;

            // API adresini giriş tipine göre ayarla
            if (girisTipi == "kullanici")
            {
                apiUrl = "https://localhost:7248/kullanici/giris"; // 
            }
            else if (girisTipi == "yonetici")
            {
                apiUrl = "https://localhost:7248/yonetici/giris"; // 
            }
            else
            {
                await DisplayAlert("Hata", "Geçersiz giriş tipi.", "Tamam");
                return;
            }



            // JSON formatında gönderilecek veri
            var girisModel = new
            {
                KullaniciAdi = kullaniciAdi,
                Sifre = sifre
            };

            string json = JsonSerializer.Serialize(girisModel);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        entryKullaniciAdi.Text = string.Empty;
                        entrySifre.Text = string.Empty;


                        if (girisTipi == "yonetici")
                        {

                            await Navigation.PushAsync(new DirectorMainPage());
                        }
                        else
                        {
                            App.GirisYapanKullaniciId = (int)await KullaniciIdGetir(KullaniciAd);
                            KullaniciId = await KullaniciIdGetir(KullaniciAd);
                            await Navigation.PushAsync(new KullaniciMainPage(KullaniciId));
                            await DisplayAlert("Başarılı", "Giriş başarılı!", "Tamam");


                            var bugunEklenenler = new List<string>();

                            var tumFilmler = await FilmleriYukle(bugunEklenenler);
                            var tumDiziler = await DizileriYukle(bugunEklenenler);

                            if (bugunEklenenler.Any())
                            {
                                string mesaj = string.Join("\n", bugunEklenenler);
                                await DisplayAlert("Bugün Eklenen İçerikler", mesaj, "Tamam");
                            }

                        }
                    }
                    else
                    {
                        await DisplayAlert("Hatalı Giriş", "Kullanıcı adı veya şifre yanlış.", "Tamam");
                    }

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
            }
        }
        public async Task<List<FilmDizi>> FilmleriYukle(List<string> bugunEklenenler)
        {
            var sonuc = new List<FilmDizi>();
            try
            {
                HttpClient client = new();
                var response = await client.GetAsync("https://localhost:7248/film/listele");

                if (!response.IsSuccessStatusCode)
                    return sonuc;

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var filmler = JsonSerializer.Deserialize<List<Film>>(json, options);

                foreach (var film in filmler)
                {
                    bool bugunEklendi = film.CikisTarihi.Date == DateTime.Today;

                    if (bugunEklendi)

                        bugunEklenenler.Add($"🎬 {film.Ad ?? "Bilinmeyen"}");


                }


            }

            catch { }

            return sonuc;
        }

        public async Task<List<FilmDizi>> DizileriYukle(List<string> bugunEklenenler)
        {
            var sonuc = new List<FilmDizi>();
            try
            {
                HttpClient client = new();
                var response = await client.GetAsync("https://localhost:7248/dizi/listele");

                if (!response.IsSuccessStatusCode)
                    return sonuc;

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var diziler = JsonSerializer.Deserialize<List<Dizi>>(json, options);

                foreach (var dizi in diziler)
                {
                    bool bugunEklendi = dizi.YuklenmeTarihi.Date == DateTime.Today;

                    if (bugunEklendi)
                        bugunEklenenler.Add($"📺 {dizi.Ad ?? "Bilinmeyen"}");

                }

            }

            catch { }

            return sonuc;
        }







        private async Task DisplayAlert(string v1, Func<string?> toString, string v2)
        {
            throw new NotImplementedException();
        }

        private void CikisYap(object sender, EventArgs e)
        {

#if ANDROID
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif WINDOWS
            System.Environment.Exit(0);
#endif
        }

    }




}