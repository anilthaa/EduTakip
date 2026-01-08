namespace EduTakip.Models
{
    public class ItemDersProgrami:ItemBase
    {
        public int Ders_ID { get; set; }
        public ItemDers Ders { get; set; }
        public DayOfWeek Gun { get; set; }
        public TimeSpan BaslangicSaati { get; set; }
        public TimeSpan BitisSaati { get; set; }
        // SinifID: EF çekirdek tarafından foreign key olarak kullanılan alan (migration mg6 ile oluştu)
        // Sinif_ID: Uygulama içi sorgularda kullanılan alan. Kaydederken bu iki alanı senkron tutuyoruz.
        public int SinifID { get; set; }
        public int Sinif_ID { get; set; }
        public ItemSinif Sinif { get; set; }
        public bool Aktif { get; set; }

    }
}
