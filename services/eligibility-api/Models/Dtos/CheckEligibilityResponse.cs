namespace EligibilityApi.Models.Dtos;
public record CheckEligibilityResponse(bool Eligible, decimal MaxAllowed, string Reason = "");