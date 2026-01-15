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
        public bool EmailConfirmed { get; set; }
        public bool Reddedildi { get; set; }
        public string? SifreSifirlamaToken { get; set; }
        public DateTime? SifreSifirlamaTokenExpiry { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailConfirmationTokenExpiry { get; set; }
    }
}
