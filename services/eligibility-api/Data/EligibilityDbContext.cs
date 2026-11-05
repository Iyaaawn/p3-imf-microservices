using Microsoft.EntityFrameworkCore;
using EligibilityApi.Models;

namespace EligibilityApi.Data;

public class EligibilityDbContext(DbContextOptions<EligibilityDbContext> options) : DbContext(options)
{
    // The CountryQuotas table
    public DbSet<CountryQuota> CountryQuotas => Set<CountryQuota>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CountryQuota>(entity =>
        {
            // Primary key: required, because EF only finds keys named "Id" or "CountryQuotaId" on its own
            entity.HasKey(q => q.CountryCode);

            // ISO codes are 3 letters; without this the column is nvarchar(max)
            entity.Property(q => q.CountryCode).HasMaxLength(3);

            // Same decimal precision as Iyan's Amount column
            entity.Property(q => q.QuotaSdrMillions).HasPrecision(18, 2);

            // Seed data: inserted by the migration
            entity.HasData(
                new CountryQuota { CountryCode = "GHA", QuotaSdrMillions = 738.0m },
                new CountryQuota { CountryCode = "KEN", QuotaSdrMillions = 542.8m },
                new CountryQuota { CountryCode = "ARG", QuotaSdrMillions = 3187.3m },
                new CountryQuota { CountryCode = "EGY", QuotaSdrMillions = 2037.1m },
                new CountryQuota { CountryCode = "PAK", QuotaSdrMillions = 2031.0m },
                new CountryQuota { CountryCode = "LKA", QuotaSdrMillions = 578.8m }
            );
        });
    }
}