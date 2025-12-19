namespace EduTakip.Models
{
    public class ItemDers:ItemBase
    {
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public int Ogretmen_ID { get; set; }
        public ItemOgretmen Ogretmen { get; set; }

        public ICollection<ItemDersKatilimci> Katilimcilar { get; set; }
        public ICollection<ItemDersProgrami> Programlar { get; set; }
        public ICollection<ItemOdev> Odevler { get; set; }
    }
}
