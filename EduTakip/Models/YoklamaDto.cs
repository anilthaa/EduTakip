namespace EduTakip.Models
{
    public class YoklamaSlotDto
    {
        public int DersProgramiId { get; set; }
        public string DersAdi { get; set; } = "";
        public string SinifAdi { get; set; } = "";
        public TimeSpan BaslangicSaati { get; set; }
        public TimeSpan BitisSaati { get; set; }
        public int ToplamOgrenci { get; set; }
        public int AlinanYoklama { get; set; }
        public bool YoklamaAlindi => AlinanYoklama > 0;
    }

    public class YoklamaOgrenciSatirDto
    {
        public int OgrenciId { get; set; }
        public string Ad { get; set; } = "";
        public string Soyad { get; set; } = "";
        public string OgrenciNo { get; set; } = "";
        public YoklamaDurumu Durum { get; set; } = YoklamaDurumu.Gelmedi;
        public int? KatilimId { get; set; }
    }

    public class YoklamaOturumDto
    {
        public int DersProgramiId { get; set; }
        public DateTime YoklamaTarihi { get; set; }
        public string DersAdi { get; set; } = "";
        public string SinifAdi { get; set; } = "";
        public TimeSpan BaslangicSaati { get; set; }
        public TimeSpan BitisSaati { get; set; }
        public int GeldiSayisi { get; set; }
        public int GelmediSayisi { get; set; }
        public int GecKaldiSayisi { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public string AlanOgretmen { get; set; } = "";
    }
}
