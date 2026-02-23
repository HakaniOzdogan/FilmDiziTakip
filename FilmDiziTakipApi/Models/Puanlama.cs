namespace FilmDiziTakipApi.Models
{
    public class Puanlama
    {
        public int Id { get; set; } // Puanlamanın benzersiz ID'si
        public int Deger { get; set; } // Puan değeri (1-10 arası gibi)
        public string? Yorum { get; set; } // Kullanıcı tarafından yapılan yorum (isteğe bağlı)

        public int KullaniciId { get; set; } // Puanlamayı yapan kullanıcının ID'si
        public Kullanici Kullanici { get; set; } // Kullanıcı nesnesi (FK)

        public int? FilmId { get; set; } // Film için yapılan puanlama
        public Film? Film { get; set; } // Film nesnesi (FK)

        public int? DiziId { get; set; } // Dizi için yapılan puanlama
        public Dizi? Dizi { get; set; } // Dizi nesnesi (FK)

        public DateTime PuanlanmaTarihi { get; set; } = DateTime.Now; // Puanlamanın yapıldığı tarih

        // Puanlama eklenirken, ilgili film veya dizinin genel puanını güncelle
       
       
    }
}