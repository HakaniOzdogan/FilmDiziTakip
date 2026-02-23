namespace FilmDiziTakipApi.Models
{
    public class Arama
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public string KategoriAd { get; set; }
        public string ImageUrl { get; set; }
        public TurEnum Tur { get; set; } = TurEnum.Film;
    }
}
