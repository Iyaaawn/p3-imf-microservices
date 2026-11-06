using RequestsApi.Models;

namespace RequestsApi.Models.Dtos;

public record CreateFundingRequestDto(string CountryCode, string Fund, decimal Amount);