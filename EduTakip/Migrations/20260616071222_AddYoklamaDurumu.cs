using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class AddYoklamaDurumu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DersKatilimlar_DersProgrami_ID",
                table: "DersKatilimlar");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GirisZamani",
                table: "DersKatilimlar",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<byte>(
                name: "Durum",
                table: "DersKatilimlar",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "YoklamaTarihi",
                table: "DersKatilimlar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_DersKatilimlar_DersProgrami_ID_Ogrenci_ID_YoklamaTarihi",
                table: "DersKatilimlar",
                columns: new[] { "DersProgrami_ID", "Ogrenci_ID", "YoklamaTarihi" },
                unique: true,
                filter: "[DelDate] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DersKatilimlar_DersProgrami_ID_Ogrenci_ID_YoklamaTarihi",
                table: "DersKatilimlar");

            migrationBuilder.DropColumn(
                name: "Durum",
                table: "DersKatilimlar");

            migrationBuilder.DropColumn(
                name: "YoklamaTarihi",
                table: "DersKatilimlar");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GirisZamani",
                table: "DersKatilimlar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DersKatilimlar_DersProgrami_ID",
                table: "DersKatilimlar",
                column: "DersProgrami_ID");
        }
    }
}
