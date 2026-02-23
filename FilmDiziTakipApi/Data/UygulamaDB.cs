using FilmDiziTakipApi.Models;
using FilmDiziTakipApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace FilmDiziTakipApi.Data
{
    public class UygulamaDB :DbContext
    {


   
    
        public UygulamaDB(DbContextOptions<UygulamaDB> options) : base(options) { }
        public DbSet<Dizi> Dizi { get; set; }
        public DbSet<Film> Film { get; set; }
        public DbSet<Kategori> Kategori { get; set; }
        public DbSet<Kullanici> Kullanici { get; set; }
        public DbSet<Puanlama> Puanlama { get; set; }
        public DbSet<Yonetici> Yonetici { get; set; }
        public DbSet<YorumFilm> YorumFilm { get; set; }
        public DbSet<YorumDizi> YorumDizi { get; set; }
        public DbSet<DiziSezon> DiziSezon { get; set; }
        public DbSet<DiziBolum> DiziBolum { get; set; }
        public DbSet<IzlemeDurumuFilm> IzlemeDurumuFilm { get; set; }
        public DbSet<DiziBolumYorumPuan> DiziBolumYorumPuan { get; set; }
        public DbSet<IzlemeDurumuDizi> IzlemeDurumuDizi { get; set; }


    }


}
