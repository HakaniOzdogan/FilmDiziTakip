using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilmDiziTakip.Modeller;
namespace FilmDiziTakip.Servisler
{
   
        public interface IDiziService
        {
            Task<List<Dizi>> TumDizileriGetirAsync();
            Task<Dizi> DiziGetirAsync(int id);
            // Ek olarak: YorumEkle, PuanVer vs. eklenecekse buraya yazılabilir
        }
    
}
