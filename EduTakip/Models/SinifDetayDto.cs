namespace EduTakip.Models
{
    public class SinifDetayDto
    {
        public int SinifId { get; set; }
        public string SinifAdi { get; set; }= string.Empty;
        public string OgretmenAdSoyad { get; set; }
        public int OgrenciSayisi { get; set; }
    }
}
