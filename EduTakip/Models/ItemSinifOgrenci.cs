namespace EduTakip.Models
{
    public class ItemSinifOgrenci:ItemBase
    {
        public int Sinif_ID { get; set; }
        public int Ogrenci_ID { get; set; }
        public DateTime KatilmaTarihi { get; set; } = DateTime.Now;
        public ItemSinif? Sinif { get; set; }
        public ItemOgrenci Ogrenci { get; set; }
    }
}
