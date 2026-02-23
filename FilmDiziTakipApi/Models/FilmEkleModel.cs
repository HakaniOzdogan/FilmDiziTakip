namespace FilmDiziTakipApi.Models
{
    public class FilmEkleModel
    {
       
        
            public string Ad { get; set; } = string.Empty;
            public string Aciklama { get; set; } = string.Empty;
            public DateTime YayınTarihi { get; set; }
            public string Yonetmen { get; set; } = string.Empty;
            public DateTime CikisTarihi { get; set; }
            public decimal? Puan { get; set; } 
            public int KategoriId { get; set; }
            public string? ImageUrl { get; set; } = string.Empty;
            public string? FilmUrl { get; set; } = string.Empty;

    }


}
