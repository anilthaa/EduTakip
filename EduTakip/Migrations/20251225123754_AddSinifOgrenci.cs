using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class AddSinifOgrenci : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SinifOgrenciler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sinif_ID = table.Column<int>(type: "int", nullable: false),
                    Ogrenci_ID = table.Column<int>(type: "int", nullable: false),
                    KatilmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SinifID = table.Column<int>(type: "int", nullable: true),
                    OgrenciID = table.Column<int>(type: "int", nullable: false),
                    CreUser = table.Column<int>(type: "int", nullable: false),
                    CreDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdUser = table.Column<int>(type: "int", nullable: true),
                    UpdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelUser = table.Column<int>(type: "int", nullable: true),
                    DelDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinifOgrenciler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SinifOgrenciler_Ogrenciler_OgrenciID",
                        column: x => x.OgrenciID,
                        principalTable: "Ogrenciler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SinifOgrenciler_Siniflar_SinifID",
                        column: x => x.SinifID,
                        principalTable: "Siniflar",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SinifOgrenciler_OgrenciID",
                table: "SinifOgrenciler",
                column: "OgrenciID");

            migrationBuilder.CreateIndex(
                name: "IX_SinifOgrenciler_SinifID",
                table: "SinifOgrenciler",
                column: "SinifID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SinifOgrenciler");
        }
    }
}
