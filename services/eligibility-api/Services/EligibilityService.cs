using EligibilityApi.Models.Dtos;
// The contract: what the service can do
namespace EligibilityApi.Services;
public interface IEligibilityService
{
    Task<CheckEligibilityResponse> CheckEligibilityAsync(CheckEligibilityRequest request);
}