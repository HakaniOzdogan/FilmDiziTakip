namespace FilmDiziTakip.Modeller
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
        public string KategoriAd { get; set; }
        public string? ImageUrl { get; set; }
        public string? DiziUrl { get; set; }
        public TurEnum Tur { get; set; } = TurEnum.Dizi;


    }
}