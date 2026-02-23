using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class IzlemeDurumuFilm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IzlemeDurumuFilm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    FilmId = table.Column<int>(type: "int", nullable: false),
                    KalanSure = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IzlemeDurumuFilm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IzlemeDurumuFilm_Film_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Film",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IzlemeDurumuFilm_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IzlemeDurumuFilm_FilmId",
                table: "IzlemeDurumuFilm",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_IzlemeDurumuFilm_KullaniciId",
                table: "IzlemeDurumuFilm",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IzlemeDurumuFilm");
        }
    }
}
