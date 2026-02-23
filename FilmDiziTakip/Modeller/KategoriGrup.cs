using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmDiziTakip.Modeller
{
    public class KategoriGrup
    {
        public string KategoriAdi { get; set; }         // Kategori adı
        public List<FilmDizi> Filmler { get; set; } = new();   // O kategoriye ait filmler/diziler
    }
}
