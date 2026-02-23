using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmDiziTakip.Modeller
{
    public class DiziBolum
    {
        public int Id { get; set; }

        public int BolumNo { get; set; }  // Örn: 1, 2, 3
        public string Ad { get; set; } = string.Empty;
        

        public string? BolumUrl { get; set; }
        // 🔗 İlişki: Hangi sezona ait?
        public int SezonId { get; set; }
        public double ortalamaPuan {  get; set; }
        public List<DiziBolumYorumPuan> Yorumlar { get; set; }
        public string ThumbnailUrl { get; set; }  // Küçük resim URL'si
        
    }
}
