using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmDiziTakip.Modeller
{
    public class FilmDiziGrup
    {
        public string GrupAdi { get; set; }               // "Filmler" veya "Diziler"
        public List<KategoriGrup> Kategoriler { get; set; } // Kategori grupları
    }
}
