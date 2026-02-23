namespace FilmDiziTakipApi.Models
{
    public class Dizi
    {
        public int Id { get; set; } 
        public string Ad { get; set; } =string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public DateTime YayınTarihi { get; set; } 
        public string Yonetmen { get; set; } =string.Empty ;
        public DateTime YuklenmeTarihi { get; set; }
        public decimal Puan { get; set; }
        public int KategoriId { get; set; }
        public string? DiziUrl { get; set; } = string.Empty;
        public Kategori Kategori { get; set; }
        public List<Puanlama> Puanlamalar { get; set; } = new List<Puanlama>(); // Diziye yapılan puanlamaların listesi
        public string? ImageUrl { get; set; }
        public ICollection<YorumDizi>? Yorumlar { get; set; }
        public TurEnum Tur { get; set; } = TurEnum.Dizi;
        public ICollection<DiziSezon> Sezonlar { get; set; } = new List<DiziSezon>();
    }
}
