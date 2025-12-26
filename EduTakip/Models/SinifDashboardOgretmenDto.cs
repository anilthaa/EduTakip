namespace EduTakip.Models
{
    public class SinifDashboardOgretmenDto
    {
        public int SinifId { get; set; }
        public string SinifAdi { get; set; }=string.Empty;
        public int OgrenciSayisi { get; set; }
        public int OdevSayisi { get; set; }
    }
}
