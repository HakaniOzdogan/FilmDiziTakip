namespace FilmDiziTakipApi.Models
{
    public class YorumFilm
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public int KullaniciId { get; set; }
        public string? YorumMetni { get; set; }
        public int? Puan { get; set; }
        public DateTime? Tarih { get; set; } = DateTime.Now;

    }
}
