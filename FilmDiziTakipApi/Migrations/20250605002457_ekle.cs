using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class ekle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SezonSayisi",
                table: "Dizi");

            migrationBuilder.CreateTable(
                name: "DiziSezon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SezonNo = table.Column<int>(type: "int", nullable: false),
                    DiziId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiziSezon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiziSezon_Dizi_DiziId",
                        column: x => x.DiziId,
                        principalTable: "Dizi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiziBolum",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BolumNo = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SezonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiziBolum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiziBolum_DiziSezon_SezonId",
                        column: x => x.SezonId,
                        principalTable: "DiziSezon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiziBolum_SezonId",
                table: "DiziBolum",
                column: "SezonId");

            migrationBuilder.CreateIndex(
                name: "IX_DiziSezon_DiziId",
                table: "DiziSezon",
                column: "DiziId");

            


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiziBolum");

           

            migrationBuilder.DropTable(
                name: "DiziSezon");

            

            migrationBuilder.AddColumn<int>(
                name: "SezonSayisi",
                table: "Dizi",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
