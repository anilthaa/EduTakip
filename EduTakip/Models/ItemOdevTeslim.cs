namespace EduTakip.Models
{
    public class ItemOdevTeslim:ItemBase
    {
        public int Odev_ID { get; set; }
        public ItemOdev Odev { get; set; }
        public int Ogrenci_ID { get; set; }
        public ItemOgrenci Ogrenci { get; set; } = null!;
        public DateTime TeslimTarihi { get; set; }
        public string DosyaYolu { get; set; }
        public int? Not { get; set; }
        public string? OgretmenYorumu { get; set; }
    }
}