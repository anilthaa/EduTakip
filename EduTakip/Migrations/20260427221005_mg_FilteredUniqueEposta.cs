using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg_FilteredUniqueEposta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_EPosta",
                table: "Kullanicilar");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_EPosta",
                table: "Kullanicilar",
                column: "EPosta",
                unique: true,
                filter: "[DelDate] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_EPosta",
                table: "Kullanicilar");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_EPosta",
                table: "Kullanicilar",
                column: "EPosta",
                unique: true);
        }
    }
}
