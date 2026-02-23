namespace FilmDiziTakipApi.Models
{
    public class DiziSezon
    {
        public int Id { get; set; }

        public int SezonNo { get; set; }  // Örn: 1. sezon

        // 🔗 Hangi diziye ait?
        public int DiziId { get; set; }
        public Dizi Dizi { get; set; } = null!;

        // 🔗 Sezondaki bölümler
        public ICollection<DiziBolum> Bolumler { get; set; } = new List<DiziBolum>();
    }

}
