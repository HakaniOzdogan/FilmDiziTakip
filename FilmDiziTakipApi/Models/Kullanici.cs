namespace FilmDiziTakipApi.Models
{
    public class Kullanici
    {
        
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    
    }
}
