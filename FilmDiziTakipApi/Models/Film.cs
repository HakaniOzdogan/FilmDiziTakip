namespace FilmDiziTakipApi.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public DateTime YayınTarihi { get; set; }
        public string Yonetmen { get; set; } = string.Empty;
        public DateTime CikisTarihi { get; set; }
        public decimal Puan { get; set; } = 0;
        public int KategoriId { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? FilmUrl { get; set; } = string.Empty;
        public Kategori Kategori { get; set; }
        public List<Puanlama> Puanlamalar { get; set; } = new List<Puanlama>(); // Filme yapılan puanlamaların listesi
        public ICollection<YorumFilm>? Yorumlar { get; set; }
        public TurEnum Tur { get; set; } = TurEnum.Film;

    }
}
