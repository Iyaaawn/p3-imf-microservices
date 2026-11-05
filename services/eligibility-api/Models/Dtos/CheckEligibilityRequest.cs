namespace EligibilityApi.Models.Dtos;
public record CheckEligibilityRequest(string CountryCode, string FundCode, decimal Amount);