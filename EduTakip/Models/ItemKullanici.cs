namespace EduTakip.Models
{
    public class ItemKullanici:ItemBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string EPosta { get; set; }
        public int KullaniciTip_ID { get; set; }
        public ItemKullaniciTip KullaniciTip { get; set; }
        public string Sifre { get; set; }
        public bool Aktif { get; set; }
    }
}
