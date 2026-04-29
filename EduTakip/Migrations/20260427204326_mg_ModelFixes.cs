using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg_ModelFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DersKatilimcilar_Kullanicilar_Ogrenci_ID",
                table: "DersKatilimcilar");

            migrationBuilder.DropForeignKey(
                name: "FK_DersKatilimlar_Kullanicilar_Ogrenci_ID",
                table: "DersKatilimlar");

            migrationBuilder.DropForeignKey(
                name: "FK_OdevTeslimler_Kullanicilar_Ogrenci_ID",
                table: "OdevTeslimler");

            migrationBuilder.DropIndex(
                name: "IX_DersProgramlari_Sinif_ID",
                table: "DersProgramlari");

            migrationBuilder.AlterColumn<int>(
                name: "Tip",
                table: "SinifKullanicilar",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OgretmenYorumu",
                table: "OdevTeslimler",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DersProgrami_ID",
                table: "DersKatilimlar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DersProgramlari_Sinif_ID_Gun_BaslangicSaati",
                table: "DersProgramlari",
                columns: new[] { "Sinif_ID", "Gun", "BaslangicSaati" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DersKatilimlar_DersProgrami_ID",
                table: "DersKatilimlar",
                column: "DersProgrami_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DersKatilimcilar_Ogrenciler_Ogrenci_ID",
                table: "DersKatilimcilar",
                column: "Ogrenci_ID",
                principalTable: "Ogrenciler",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DersKatilimlar_DersProgramlari_DersProgrami_ID",
                table: "DersKatilimlar",
                column: "DersProgrami_ID",
                principalTable: "DersProgramlari",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DersKatilimlar_Ogrenciler_Ogrenci_ID",
                table: "DersKatilimlar",
                column: "Ogrenci_ID",
                principalTable: "Ogrenciler",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OdevTeslimler_Ogrenciler_Ogrenci_ID",
                table: "OdevTeslimler",
                column: "Ogrenci_ID",
                principalTable: "Ogrenciler",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DersKatilimcilar_Ogrenciler_Ogrenci_ID",
                table: "DersKatilimcilar");

            migrationBuilder.DropForeignKey(
                name: "FK_DersKatilimlar_DersProgramlari_DersProgrami_ID",
                table: "DersKatilimlar");

            migrationBuilder.DropForeignKey(
                name: "FK_DersKatilimlar_Ogrenciler_Ogrenci_ID",
                table: "DersKatilimlar");

            migrationBuilder.DropForeignKey(
                name: "FK_OdevTeslimler_Ogrenciler_Ogrenci_ID",
                table: "OdevTeslimler");

            migrationBuilder.DropIndex(
                name: "IX_DersProgramlari_Sinif_ID_Gun_BaslangicSaati",
                table: "DersProgramlari");

            migrationBuilder.DropIndex(
                name: "IX_DersKatilimlar_DersProgrami_ID",
                table: "DersKatilimlar");

            migrationBuilder.DropColumn(
                name: "DersProgrami_ID",
                table: "DersKatilimlar");

            migrationBuilder.AlterColumn<string>(
                name: "Tip",
                table: "SinifKullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "OgretmenYorumu",
                table: "OdevTeslimler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DersProgramlari_Sinif_ID",
                table: "DersProgramlari",
                column: "Sinif_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DersKatilimcilar_Kullanicilar_Ogrenci_ID",
                table: "DersKatilimcilar",
                column: "Ogrenci_ID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DersKatilimlar_Kullanicilar_Ogrenci_ID",
                table: "DersKatilimlar",
                column: "Ogrenci_ID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OdevTeslimler_Kullanicilar_Ogrenci_ID",
                table: "OdevTeslimler",
                column: "Ogrenci_ID",
                principalTable: "Kullanicilar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
