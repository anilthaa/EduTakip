using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sinif",
                table: "Ogrenciler");

            migrationBuilder.AddColumn<int>(
                name: "Sinif_ID",
                table: "Ogrenciler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_Sinif_ID",
                table: "Ogrenciler",
                column: "Sinif_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ogrenciler_Siniflar_Sinif_ID",
                table: "Ogrenciler",
                column: "Sinif_ID",
                principalTable: "Siniflar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ogrenciler_Siniflar_Sinif_ID",
                table: "Ogrenciler");

            migrationBuilder.DropIndex(
                name: "IX_Ogrenciler_Sinif_ID",
                table: "Ogrenciler");

            migrationBuilder.DropColumn(
                name: "Sinif_ID",
                table: "Ogrenciler");

            migrationBuilder.AddColumn<string>(
                name: "Sinif",
                table: "Ogrenciler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
