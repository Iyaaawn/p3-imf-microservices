using Microsoft.EntityFrameworkCore;
using CountriesApi.Models;
using System.Reflection.Metadata;

namespace CountriesApi.Data;

//Code First Approach to making the RequestDb
public class CountriesDbContext(DbContextOptions<CountriesDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();

    protected override void OnModelCreating(ModelBuilder b) //"When the model is created, it must have these"
    {
        b.Entity<Country>(e =>
        {
            //Property Constraints
            e.HasKey(c => c.IsoCode);
            e.Property(c => c.IsoCode).HasMaxLength(3);
            
            //Seeded data (here temporarily for when you're testing the approve/reject status changes)
            e.HasData(
                new Country{IsoCode = "GHA", Name = "Ghana", IsActive = true},
                new Country{IsoCode = "KEN", Name = "Kenya", IsActive = true},
                new Country{IsoCode = "ARG", Name = "Argentina", IsActive = true},
                new Country{IsoCode = "EGY", Name = "Egypt", IsActive = true},
                new Country{IsoCode = "PAK", Name = "Pakistan", IsActive = false},
                new Country{IsoCode = "LKA", Name = "Sri Lanka", IsActive = true}                
                );
        });
    }
}