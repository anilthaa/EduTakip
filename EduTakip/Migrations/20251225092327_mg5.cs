using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SinifKullanicilar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sinif_ID = table.Column<int>(type: "int", nullable: false),
                    Kullanici_ID = table.Column<int>(type: "int", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinifKullanicilar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SinifKullanicilar_Kullanicilar_Kullanici_ID",
                        column: x => x.Kullanici_ID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SinifKullanicilar_Siniflar_Sinif_ID",
                        column: x => x.Sinif_ID,
                        principalTable: "Siniflar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SinifKullanicilar_Kullanici_ID",
                table: "SinifKullanicilar",
                column: "Kullanici_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SinifKullanicilar_Sinif_ID",
                table: "SinifKullanicilar",
                column: "Sinif_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SinifKullanicilar");
        }
    }
}
