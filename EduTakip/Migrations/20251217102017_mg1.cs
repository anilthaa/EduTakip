using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Birimler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Birimler", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciTipleri",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciTipleri", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Soyadi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EPosta = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KullaniciTip_ID = table.Column<int>(type: "int", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_KullaniciTipleri_KullaniciTip_ID",
                        column: x => x.KullaniciTip_ID,
                        principalTable: "KullaniciTipleri",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ogrenciler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kullanici_ID = table.Column<int>(type: "int", nullable: false),
                    OgrenciNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sinif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ogrenciler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ogrenciler_Kullanicilar_Kullanici_ID",
                        column: x => x.Kullanici_ID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ogretmenler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kullanici_ID = table.Column<int>(type: "int", nullable: false),
                    Birim_ID = table.Column<int>(type: "int", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ogretmenler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ogretmenler_Birimler_Birim_ID",
                        column: x => x.Birim_ID,
                        principalTable: "Birimler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ogretmenler_Kullanicilar_Kullanici_ID",
                        column: x => x.Kullanici_ID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dersler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ogretmen_ID = table.Column<int>(type: "int", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dersler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Dersler_Ogretmenler_Ogretmen_ID",
                        column: x => x.Ogretmen_ID,
                        principalTable: "Ogretmenler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DersKatilimcilar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ders_ID = table.Column<int>(type: "int", nullable: false),
                    Ogrenci_ID = table.Column<int>(type: "int", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersKatilimcilar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DersKatilimcilar_Dersler_Ders_ID",
                        column: x => x.Ders_ID,
                        principalTable: "Dersler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DersKatilimcilar_Kullanicilar_Ogrenci_ID",
                        column: x => x.Ogrenci_ID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DersKatilimlar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ders_ID = table.Column<int>(type: "int", nullable: false),
                    Ogrenci_ID = table.Column<int>(type: "int", nullable: false),
                    GirisZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CikisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersKatilimlar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DersKatilimlar_Dersler_Ders_ID",
                        column: x => x.Ders_ID,
                        principalTable: "Dersler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DersKatilimlar_Kullanicilar_Ogrenci_ID",
                        column: x => x.Ogrenci_ID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DersProgramlari",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ders_ID = table.Column<int>(type: "int", nullable: false),
                    Gun = table.Column<int>(type: "int", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    Sinif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersProgramlari", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DersProgramlari_Dersler_Ders_ID",
                        column: x => x.Ders_ID,
                        principalTable: "Dersler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Odevler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ders_ID = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerilisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonTeslimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Odevler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Odevler_Dersler_Ders_ID",
                        column: x => x.Ders_ID,
                        principalTable: "Dersler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OdevTeslimler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Odev_ID = table.Column<int>(type: "int", nullable: false),
                    Ogrenci_ID = table.Column<int>(type: "int", nullable: false),
                    TeslimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DosyaYolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Not = table.Column<int>(type: "int", nullable: true),
                    OgretmenYorumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdevTeslimler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OdevTeslimler_Kullanicilar_Ogrenci_ID",
                        column: x => x.Ogrenci_ID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OdevTeslimler_Odevler_Odev_ID",
                        column: x => x.Odev_ID,
                        principalTable: "Odevler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DersKatilimcilar_Ders_ID",
                table: "DersKatilimcilar",
                column: "Ders_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DersKatilimcilar_Ogrenci_ID",
                table: "DersKatilimcilar",
                column: "Ogrenci_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DersKatilimlar_Ders_ID",
                table: "DersKatilimlar",
                column: "Ders_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DersKatilimlar_Ogrenci_ID",
                table: "DersKatilimlar",
                column: "Ogrenci_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Dersler_Ogretmen_ID",
                table: "Dersler",
                column: "Ogretmen_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DersProgramlari_Ders_ID_Gun_BaslangicSaati_BitisSaati",
                table: "DersProgramlari",
                columns: new[] { "Ders_ID", "Gun", "BaslangicSaati", "BitisSaati" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_EPosta",
                table: "Kullanicilar",
                column: "EPosta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciTip_ID",
                table: "Kullanicilar",
                column: "KullaniciTip_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Odevler_Ders_ID",
                table: "Odevler",
                column: "Ders_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OdevTeslimler_Odev_ID",
                table: "OdevTeslimler",
                column: "Odev_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OdevTeslimler_Ogrenci_ID",
                table: "OdevTeslimler",
                column: "Ogrenci_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_Kullanici_ID",
                table: "Ogrenciler",
                column: "Kullanici_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_OgrenciNo",
                table: "Ogrenciler",
                column: "OgrenciNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ogretmenler_Birim_ID",
                table: "Ogretmenler",
                column: "Birim_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ogretmenler_Kullanici_ID",
                table: "Ogretmenler",
                column: "Kullanici_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DersKatilimcilar");

            migrationBuilder.DropTable(
                name: "DersKatilimlar");

            migrationBuilder.DropTable(
                name: "DersProgramlari");

            migrationBuilder.DropTable(
                name: "OdevTeslimler");

            migrationBuilder.DropTable(
                name: "Ogrenciler");

            migrationBuilder.DropTable(
                name: "Odevler");

            migrationBuilder.DropTable(
                name: "Dersler");

            migrationBuilder.DropTable(
                name: "Ogretmenler");

            migrationBuilder.DropTable(
                name: "Birimler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "KullaniciTipleri");
        }
    }
}
