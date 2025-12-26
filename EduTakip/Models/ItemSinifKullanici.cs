namespace EduTakip.Models
{
    public class ItemSinifKullanici:ItemBase
    {
        public int Sinif_ID { get; set; }
        public ItemSinif Sinif { get; set; }
        public int Kullanici_ID { get; set; }
        public ItemKullanici Kullanici { get; set; }
        public string Tip { get; set; }
    }
}
