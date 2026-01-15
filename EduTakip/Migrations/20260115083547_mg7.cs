using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTakip.Migrations
{
    /// <inheritdoc />
    public partial class mg7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SifreSifirlamaToken",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SifreSifirlamaTokenExpiry",
                table: "Kullanicilar",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SifreSifirlamaToken",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "SifreSifirlamaTokenExpiry",
                table: "Kullanicilar");
        }
    }
}
