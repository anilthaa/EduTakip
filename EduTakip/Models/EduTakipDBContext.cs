using Microsoft.EntityFrameworkCore;

namespace EduTakip.Models
{
    public class EduTakipDBContext:DbContext
    {
        public EduTakipDBContext(DbContextOptions<EduTakipDBContext> options) : base(options)
        {
        }
        public DbSet<ItemKullanici> Kullanicilar { get; set; }
        public DbSet<ItemKullaniciTip> KullaniciTipleri { get; set; }
        public DbSet<ItemOgrenci> Ogrenciler { get; set; }
        public DbSet<ItemOgretmen> Ogretmenler { get; set; }
        public DbSet<ItemBirim> Birimler { get; set; }
        public DbSet<ItemSinif> Siniflar { get; set; }
        public DbSet<ItemSinifKullanici> SinifKullanicilar { get; set; }
        public DbSet<ItemSinifOgrenci> SinifOgrenciler { get; set; }
        public DbSet<ItemDers> Dersler { get; set; }
        public DbSet<ItemDersKatilimci> DersKatilimcilar { get; set; }
        public DbSet<ItemDersKatilim> DersKatilimlar { get; set; }
        public DbSet<ItemDersProgrami> DersProgramlari { get; set; }
        public DbSet<ItemOdev> Odevler { get; set; }
        public DbSet<ItemOdevTeslim> OdevTeslimler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft Delete için global query filter - Tüm ItemBase'den türeyen entity'ler için
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ItemBase).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ItemBase.DelDate));
                    var nullConstant = System.Linq.Expressions.Expression.Constant(null, typeof(DateTime?));
                    var condition = System.Linq.Expressions.Expression.Equal(property, nullConstant);
                    var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);
                    
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            modelBuilder.Entity<ItemKullanici>()
                .HasIndex(x => x.EPosta)
                .IsUnique();

            modelBuilder.Entity<ItemKullanici>()
                .HasIndex(x => x.EPosta)
                .IsUnique();

            modelBuilder.Entity<ItemKullanici>()
                .HasOne(x => x.KullaniciTip)
                .WithMany()
                .HasForeignKey(x => x.KullaniciTip_ID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ItemOgrenci>()
                .HasOne(x => x.Kullanici)
                .WithMany()
                .HasForeignKey(x => x.Kullanici_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemOgrenci>()
                .HasOne(x => x.Sinif)
                .WithMany()
                .HasForeignKey(x => x.Sinif_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemOgrenci>()
                .HasIndex(x => x.OgrenciNo)
                .IsUnique();


            modelBuilder.Entity<ItemOgretmen>()
                .HasOne(x => x.Kullanici)
                .WithMany()
                .HasForeignKey(x => x.Kullanici_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemOgretmen>()
                .HasOne(x => x.Birim)
                .WithMany()
                .HasForeignKey(x => x.Birim_ID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ItemDers>()
                .HasOne(x => x.Ogretmen)
                .WithMany()
                .HasForeignKey(x => x.Ogretmen_ID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ItemDersKatilimci>()
                .HasOne(x => x.Ders)
                .WithMany(x => x.Katilimcilar)
                .HasForeignKey(x => x.Ders_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemDersKatilimci>()
                .HasOne(x => x.Ogrenci)
                .WithMany()
                .HasForeignKey(x => x.Ogrenci_ID)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<ItemDersKatilim>()
               .HasOne(x => x.Ders)
               .WithMany()
               .HasForeignKey(x => x.Ders_ID)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemDersKatilim>()
                .HasOne(x => x.Ogrenci)
                .WithMany()
                .HasForeignKey(x => x.Ogrenci_ID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ItemDersProgrami>()
               .HasOne(x => x.Ders)
               .WithMany(x => x.Programlar)
               .HasForeignKey(x => x.Ders_ID)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemDersProgrami>()
                .HasIndex(x => new { x.Ders_ID, x.Gun, x.BaslangicSaati, x.BitisSaati })
                .IsUnique();


            modelBuilder.Entity<ItemOdev>()
                .HasOne(x => x.Ders)
                .WithMany(x => x.Odevler)
                .HasForeignKey(x => x.Ders_ID)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<ItemOdevTeslim>()
                .HasOne(x => x.Odev)
                .WithMany()
                .HasForeignKey(x => x.Odev_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemOdevTeslim>()
                .HasOne(x => x.Ogrenci)
                .WithMany()
                .HasForeignKey(x => x.Ogrenci_ID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemSinifKullanici>()
                .HasOne(x => x.Sinif)
                .WithMany()
                .HasForeignKey(x => x.Sinif_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemSinifKullanici>()
                .HasOne(x => x.Kullanici)
                .WithMany()
                .HasForeignKey(x => x.Kullanici_ID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
