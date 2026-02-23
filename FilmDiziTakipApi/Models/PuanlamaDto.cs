
namespace FilmDiziTakipApi.Models.Dtos
{
    public class PuanlamaDto
    {
        public int Deger { get; set; }
        public string? Yorum { get; set; }
        public int KullaniciId { get; set; }
        public int? FilmId { get; set; }
        public int? DiziId { get; set; }
    }
}
