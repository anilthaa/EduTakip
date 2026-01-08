using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sinif",
                table: "DersProgramlari");

            migrationBuilder.AddColumn<int>(
                name: "SinifID",
                table: "DersProgramlari",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sinif_ID",
                table: "DersProgramlari",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DersProgramlari_SinifID",
                table: "DersProgramlari",
                column: "SinifID");

            migrationBuilder.AddForeignKey(
                name: "FK_DersProgramlari_Siniflar_SinifID",
                table: "DersProgramlari",
                column: "SinifID",
                principalTable: "Siniflar",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DersProgramlari_Siniflar_SinifID",
                table: "DersProgramlari");

            migrationBuilder.DropIndex(
                name: "IX_DersProgramlari_SinifID",
                table: "DersProgramlari");

            migrationBuilder.DropColumn(
                name: "SinifID",
                table: "DersProgramlari");

            migrationBuilder.DropColumn(
                name: "Sinif_ID",
                table: "DersProgramlari");

            migrationBuilder.AddColumn<string>(
                name: "Sinif",
                table: "DersProgramlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
