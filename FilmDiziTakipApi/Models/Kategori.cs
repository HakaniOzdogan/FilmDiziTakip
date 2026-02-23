namespace FilmDiziTakipApi.Models
{
    public class Kategori
    {
        public int Id { get; set; } // Kategorinin benzersiz ID'si
        public string Ad { get; set; } = string.Empty; // Kategori adı (Örn: Aksiyon, Komedi)

        // Film ve Dizi ile ilişkiler
    }
}