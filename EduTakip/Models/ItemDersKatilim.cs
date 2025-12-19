namespace EduTakip.Models
{
    public class ItemDersKatilim:ItemBase
    {
        public int Ders_ID { get; set; }
        public ItemDers Ders { get; set; }
        public int Ogrenci_ID { get; set; }
        public ItemKullanici Ogrenci { get; set; }
        public DateTime GirisZamani { get; set; }
        public DateTime? CikisZamani { get; set; }
    }
}
