using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace FilmDiziTakipApi.Data
{
    public class UygulamaDBFactory : IDesignTimeDbContextFactory<UygulamaDB>
    {
        public UygulamaDB CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UygulamaDB>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FilmDiziTakipDb;Trusted_Connection=True;");

            return new UygulamaDB(optionsBuilder.Options);
        }
    }
}
