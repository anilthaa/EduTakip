namespace EduTakip.Models
{
    public class SinifOdevListDto
    {
        public int OdevId { get; set; }
        public string Baslik { get; set; }
        public DateTime SonTeslimTarihi { get; set; }
        public int ToplamOgrenci { get; set; }
        public int TeslimEden { get; set; }
        public int TeslimEtmeyen => ToplamOgrenci - TeslimEden;
    }
}
