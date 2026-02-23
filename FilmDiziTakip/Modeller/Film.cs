using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmDiziTakip.Modeller
{
    public class Film
    {
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? Aciklama { get; set; }
        public DateTime YayınTarihi { get; set; }
        public string? Yonetmen { get; set; }
        public DateTime CikisTarihi { get; set; }
        public int KategoriId { get; set; }
        public string? ImageUrl { get; set; }
        public string? FilmUrl { get; set; }
        public TurEnum Tur { get; set; } = TurEnum.Film;

        public string KategoriAd { get; set; } = string.Empty;

    }

}
