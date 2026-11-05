using Microsoft.EntityFrameworkCore;
using EligibilityApi.Data;
using EligibilityApi.Models.Dtos;
namespace EligibilityApi.Services;

public class EligibilityService : IEligibilityService
{
    private readonly EligibilityDbContext _db;
    public EligibilityService(EligibilityDbContext db)
    {
        _db = db;
    }

     public async Task<CheckEligibilityResponse> CheckEligibilityAsync(CheckEligibilityRequest request)
    {
        // Look up the country's quota
        var quota = await _db.CountryQuotas
            .FirstOrDefaultAsync(q => q.CountryCode == request.CountryCode);

        // Unknown country: not eligible
        if (quota is null)
            return new CheckEligibilityResponse(false, 0, $"No quota on file for {request.CountryCode}");
        // The rule: max is 200% of quota
        var maxAllowed = quota.QuotaSdrMillions * 2;
        var eligible = request.Amount <= maxAllowed;

        var reason = eligible
            ? "Within 200% of quota"
            : $"Exceeds 200% of quota (max {maxAllowed}m SDR)";

        return new CheckEligibilityResponse(eligible, maxAllowed, reason);
    }
}