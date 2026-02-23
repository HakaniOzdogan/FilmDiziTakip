using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameSutunAdi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tur",
                table: "Film");

            migrationBuilder.DropColumn(
                name: "Tur",
                table: "Dizi");

            migrationBuilder.AddColumn<DateTime>(
                name: "CikisTarihi",
                table: "Film",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "YuklenmeTarihi",
                table: "Dizi",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CikisTarihi",
                table: "Film");

            migrationBuilder.DropColumn(
                name: "YuklenmeTarihi",
                table: "Dizi");

            migrationBuilder.AddColumn<string>(
                name: "Tur",
                table: "Film",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tur",
                table: "Dizi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
