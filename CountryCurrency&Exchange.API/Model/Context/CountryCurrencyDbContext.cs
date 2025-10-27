using CountryCurrency_Exchange.API.Model.CountryModel;
using Microsoft.EntityFrameworkCore;

namespace CountryCurrency_Exchange.API.Model.Context
{
    public class CountryCurrencyDbContext : DbContext
    {
        public CountryCurrencyDbContext(DbContextOptions<CountryCurrencyDbContext> options) : base(options)
        {
        }
        public DbSet<Country> Countries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(c => c.ExchangeRate)
                      .HasPrecision(18, 2);   // up to 999 trillion, 2 decimal places
                entity.Property(c => c.EstimatedGdp)
                      .HasPrecision(18, 2);   // same here
            });

            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Nigeria",
                    Capital = "Abuja",
                    Region = "Africa",
                    Population = 206139589,
                    CurrencyCode = "NGN",
                    ExchangeRate = 1600.2m,
                    EstimatedGdp = 25767448125.2m,
                    FlagUrl = "https://flagcdn.com/ng.svg",
                    LastRefreshedAt = DateTime.UtcNow
                },
                new Country
                {
                    Id = 2,
                    Name = "Ghana",
                    Capital = "Accra",
                    Region = "Africa",
                    Population = 31072940,
                    CurrencyCode = "GHS",
                    ExchangeRate = 15.34m,
                    EstimatedGdp = 3029834520.6m,
                    FlagUrl = "https://flagcdn.com/gh.svg",
                    LastRefreshedAt = DateTime.UtcNow
                }
            );
        }

    }
}
