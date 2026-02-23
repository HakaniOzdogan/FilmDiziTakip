namespace FilmDiziTakipApi.Models
{
    public class IzlemeDurumuDizi
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int DiziBolumId { get; set; }
        public double IzlenmeSuresi { get; set; } // saniye cinsinden
        public DateTime IzlenmeTarihi { get; set; }
        public DiziBolum DiziBolum { get; set; }  // navigation property
    }

}
