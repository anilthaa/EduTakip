namespace EduTakip.Models
{
    public class ItemDersKatilimci:ItemBase
    {
        public int Ders_ID { get; set; }
        public ItemDers Ders { get; set; }
        public int Ogrenci_ID { get; set; }
        public ItemKullanici Ogrenci { get; set; }
    }
}
