namespace EduTakip.Models
{
    public class ItemSinifOgrenci : ItemBase
    {
        public int Sinif_ID { get; set; }
        public ItemSinif Sinif { get; set; } = null!;
        public int Ogrenci_ID { get; set; }
        public ItemOgrenci Ogrenci { get; set; } = null!;
        public DateTime KatilmaTarihi { get; set; }
    }
}