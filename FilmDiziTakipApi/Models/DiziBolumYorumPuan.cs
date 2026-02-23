namespace FilmDiziTakipApi.Models
{
    public class DiziBolumYorumPuan
    {
        public int Id { get; set; }

        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }

        public int DiziBolumId { get; set; }
        public DiziBolum DiziBolum { get; set; }

        public int Puan { get; set; } // 1-5 arasında
        public string? Yorum { get; set; }

        public DateTime YorumTarihi { get; set; } = DateTime.Now;
    }

}
