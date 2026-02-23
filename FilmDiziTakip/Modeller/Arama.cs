using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmDiziTakip.Modeller
{
    public class Arama
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public string KategoriAd { get; set; }
        public string ImageUrl { get; set; }
        public TurEnum Tur { get; set; }
    }
}
