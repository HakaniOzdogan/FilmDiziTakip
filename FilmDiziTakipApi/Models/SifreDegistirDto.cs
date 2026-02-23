namespace FilmDiziTakipApi.Models
{
    public class SifreDegistirDto
    {
        public int KullaniciId { get; set; }
        public string EskiSifre { get; set; } = string.Empty;
        public string YeniSifre { get; set; } = string.Empty;
    }

}
