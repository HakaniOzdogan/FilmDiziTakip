namespace FilmDiziTakipApi.Models
{
    public class Yonetici
    {
        public int Id { get; set; }
        public string YoneticiAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

    }
}
