using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmDiziTakipApi.Migrations
{
    /// <inheritdoc />
    public partial class YeniModellerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Kullanicilar",
                table: "Kullanicilar");

            migrationBuilder.RenameTable(
                name: "Kullanicilar",
                newName: "Kullanici");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kullanici",
                table: "Kullanici",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Kategori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategori", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Yonetici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YoneticiAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yonetici", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dizi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YayınTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Yonetmen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SezonSayisi = table.Column<int>(type: "int", nullable: false),
                    Puan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dizi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dizi_Kategori_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Film",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YayınTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Yonetmen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Film", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Film_Kategori_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Puanlama",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deger = table.Column<int>(type: "int", nullable: false),
                    Yorum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    FilmId = table.Column<int>(type: "int", nullable: true),
                    DiziId = table.Column<int>(type: "int", nullable: true),
                    PuanlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puanlama", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puanlama_Dizi_DiziId",
                        column: x => x.DiziId,
                        principalTable: "Dizi",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Puanlama_Film_FilmId",
                        column: x => x.FilmId,
                        principalTable: "Film",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Puanlama_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dizi_KategoriId",
                table: "Dizi",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Film_KategoriId",
                table: "Film",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Puanlama_DiziId",
                table: "Puanlama",
                column: "DiziId");

            migrationBuilder.CreateIndex(
                name: "IX_Puanlama_FilmId",
                table: "Puanlama",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_Puanlama_KullaniciId",
                table: "Puanlama",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Puanlama");

            migrationBuilder.DropTable(
                name: "Yonetici");

            migrationBuilder.DropTable(
                name: "Dizi");

            migrationBuilder.DropTable(
                name: "Film");

            migrationBuilder.DropTable(
                name: "Kategori");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kullanici",
                table: "Kullanici");

            migrationBuilder.RenameTable(
                name: "Kullanici",
                newName: "Kullanicilar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kullanicilar",
                table: "Kullanicilar",
                column: "Id");
        }
    }
}
