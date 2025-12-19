namespace EduTakip.Models
{
    public class ItemDersProgrami:ItemBase
    {
        public int Ders_ID { get; set; }
        public ItemDers Ders { get; set; }
        public DayOfWeek Gun { get; set; }
        public TimeSpan BaslangicSaati { get; set; }
        public TimeSpan BitisSaati { get; set; }
        public string Sinif { get; set; }
        public bool Aktif { get; set; }

    }
}
