namespace EduTakip.Models
{
    public class ItemOgretmen:ItemBase
    {
        public int Kullanici_ID { get; set; }
        public ItemKullanici Kullanici { get; set; }
        public int Birim_ID { get; set; }
        public ItemBirim Birim { get; set; }
    }
}
