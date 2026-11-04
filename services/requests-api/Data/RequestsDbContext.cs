using Microsoft.EntityFrameworkCore;
using RequestsApi.Models;

namespace RequestsApi.Data;

//Code First Approach to making the RequestDb
public class RequestsDbContext(DbContextOptions<RequestsDbContext> options) : DbContext(options)
{
    public DbSet<FundingRequest> FundingRequests => Set<FundingRequest>();

    protected override void OnModelCreating(ModelBuilder b) //"When the model is created, it must have these"
    {
        b.Entity<FundingRequest>(e =>
        {
            //Property Constraints
            e.Property(r => r.Status).HasConversion<string>(); //The Status property has to be a string
            e.Property(r => r.Amount).HasPrecision(18, 2); //The Amount property has to be a ______ (idk)
            
            //Seeded data (here temporarily for when you're testing the approve/reject status changes)
            e.HasData(
                new FundingRequest { Id = 1, CountryCode = "GHA", CountryName = "Ghana", Fund = "ECF", Amount = 900, Status = RequestStatus.Submitted, CreatedAt = new DateTime(2026, 11, 3, 0, 0, 0, DateTimeKind.Utc) },
                new FundingRequest { Id = 2, CountryCode = "KEN", CountryName = "Kenya", Fund = "EFF", Amount = 1000, Status = RequestStatus.Submitted, CreatedAt = new DateTime(2026, 11, 3, 0, 0, 0, DateTimeKind.Utc) }
                );
        });
    }
}