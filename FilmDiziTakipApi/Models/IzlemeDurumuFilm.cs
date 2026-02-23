namespace FilmDiziTakipApi.Models
{
    public class IzlemeDurumuFilm
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int FilmId { get; set; }
        public double KalanSure { get; set; }
        public DateTime YayinlanmaTarihi { get; set; } = DateTime.Now;


        public Kullanici Kullanici { get; set; }
        public Film Film { get; set; }
    }

}
