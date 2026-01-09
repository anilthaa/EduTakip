namespace EduTakip.Models
{
    public class ItemOgrenci:ItemBase
    {
        public int Kullanici_ID { get; set; }
        public ItemKullanici Kullanici { get; set; } = null!;
        public string OgrenciNo { get; set; } = string.Empty;
        public int? Sinif_ID { get; set; }
        public ItemSinif? Sinif { get; set; }
    }
}
