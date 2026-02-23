using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class BolumUrlEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BolumUrl",
                table: "DiziBolum",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BolumUrl",
                table: "DiziBolum");
        }
    }
}
