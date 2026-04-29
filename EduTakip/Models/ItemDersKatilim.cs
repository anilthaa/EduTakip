namespace EduTakip.Models
{
    public class ItemDersKatilim:ItemBase
    {
        public int Ders_ID { get; set; }
        public ItemDers Ders { get; set; }
        public int DersProgrami_ID { get; set; }
        public ItemDersProgrami DersProgrami { get; set; } = null!;
        public int Ogrenci_ID { get; set; }
        public ItemOgrenci Ogrenci { get; set; } = null!;
        public DateTime GirisZamani { get; set; }
        public DateTime? CikisZamani { get; set; }
    }
}