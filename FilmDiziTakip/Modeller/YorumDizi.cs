using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmDiziTakip.Modeller
{
    internal class YorumDizi
    {
        public int Id { get; set; }
        public int DiziId { get; set; }
        public int? KullaniciId { get; set; }
        
        public string? YorumMetni { get; set; }
        public int Puan { get; set; }
        public DateTime? Tarih { get; set; } = DateTime.Now;
    }
}
