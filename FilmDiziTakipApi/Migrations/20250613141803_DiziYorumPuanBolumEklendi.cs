using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class DiziYorumPuanBolumEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ortalamaPuan",
                table: "DiziBolum",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "DiziBolumYorumPuan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    DiziBolumId = table.Column<int>(type: "int", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    Yorum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YorumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiziBolumYorumPuan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiziBolumYorumPuan_DiziBolum_DiziBolumId",
                        column: x => x.DiziBolumId,
                        principalTable: "DiziBolum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiziBolumYorumPuan_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiziBolumYorumPuan_DiziBolumId",
                table: "DiziBolumYorumPuan",
                column: "DiziBolumId");

            migrationBuilder.CreateIndex(
                name: "IX_DiziBolumYorumPuan_KullaniciId",
                table: "DiziBolumYorumPuan",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiziBolumYorumPuan");

            migrationBuilder.DropColumn(
                name: "ortalamaPuan",
                table: "DiziBolum");
        }
    }
}
