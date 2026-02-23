using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class SezonNoEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SezonNo",
                table: "DiziBolum",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SezonNo",
                table: "DiziBolum");
        }
    }
}
