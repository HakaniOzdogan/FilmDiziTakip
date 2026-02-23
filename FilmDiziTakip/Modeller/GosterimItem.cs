namespace FilmDiziTakip.Modeller
{
    public class GosterimItem
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsFilm { get; set; }

        public string KategoriAd { get; set; } = string.Empty;  // EKLEDİM
        public string? Url { get; set; }  // İstersen FilmUrl ya da DiziUrl için
    }
}
