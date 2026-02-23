using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class IzlemeDurumuDiziEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IzlemeDurumuDizi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    DiziBolumId = table.Column<int>(type: "int", nullable: false),
                    IzlenmeSuresi = table.Column<double>(type: "float", nullable: false),
                    IzlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IzlemeDurumuDizi", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IzlemeDurumuDizi");
        }
    }
}
