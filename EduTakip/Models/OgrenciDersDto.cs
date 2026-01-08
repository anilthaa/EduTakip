namespace EduTakip.Models
{
    public class OgrenciDersDto
    {
        public int DersId { get; set; }
        public string DersAdi { get; set; }
        public string OgretmenAdSoyad { get; set; }
        public List<string> Siniflar { get; set; }
    }
}
