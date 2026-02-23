namespace FilmDiziTakipApi.Models
{
    public class DiziBolum
    {
        public int Id { get; set; }

        public int BolumNo { get; set; }  // Örn: 1, 2, 3
        public string Ad { get; set; } = string.Empty;
        
        public string? BolumUrl { get; set; } = string.Empty;

        // 🔗 İlişki: Hangi sezona ait?
        public int SezonId { get; set; }
        public DiziSezon Sezon { get; set; } = null!;
        public double ortalamaPuan { get; set; } = 0;
        public List<DiziBolumYorumPuan> Yorumlar { get; set; }
        public string ThumbnailUrl { get; set; }  // Küçük resim URL'si
    }

}
