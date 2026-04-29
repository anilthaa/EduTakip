namespace EduTakip.Models
{
    public class ItemOdev:ItemBase
    {
        public int Ders_ID { get; set; }
        public ItemDers Ders { get; set; }
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public DateTime VerilisTarihi { get; set; }
        public DateTime SonTeslimTarihi { get; set; }
        public ICollection<ItemOdevTeslim> Teslimler { get; set; }
    }
}
