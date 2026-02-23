using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class izlemedurumunasütunekle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IzlemeDurumuDizi_DiziBolumId",
                table: "IzlemeDurumuDizi",
                column: "DiziBolumId");

            migrationBuilder.AddForeignKey(
                name: "FK_IzlemeDurumuDizi_DiziBolum_DiziBolumId",
                table: "IzlemeDurumuDizi",
                column: "DiziBolumId",
                principalTable: "DiziBolum",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IzlemeDurumuDizi_DiziBolum_DiziBolumId",
                table: "IzlemeDurumuDizi");

            migrationBuilder.DropIndex(
                name: "IX_IzlemeDurumuDizi_DiziBolumId",
                table: "IzlemeDurumuDizi");
        }
    }
}
