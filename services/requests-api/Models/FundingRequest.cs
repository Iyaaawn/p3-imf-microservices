namespace RequestsApi.Models;

public enum RequestStatus { Submitted, PendingEligibility, Approved, Rejected }

public class FundingRequest
{
    public int Id { get; set; }
    public string CountryCode { get; set; } = "";   // plain value: no navigation property, no FK
    public string CountryName { get; set; } = "";   // snapshot copied from the Countries API
    public string Fund { get; set; } = "";          // SBA, EFF, ECF, FCL, RSF
    public decimal Amount { get; set; }
    public RequestStatus Status { get; set; }
    public string? EligibilityReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DecidedAt { get; set; }
}