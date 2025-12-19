namespace EduTakip.Models
{
    public class ItemOgrenci:ItemBase
    {
        public int Kullanici_ID { get; set; }
        public ItemKullanici Kullanici { get; set; }
        public string OgrenciNo { get; set; }
        public string Sinif { get; set; }
    }
}
