using Microsoft.AspNetCore.Mvc;
using EligibilityApi.Models.Dtos;
using EligibilityApi.Services;

namespace EligibilityApi.Controllers;

[ApiController]                    // automatic 400s for bad JSON, binds the body for you
[Route("api/eligibility")]         // base route for this controller
public class EligibilityController(IEligibilityService service) : ControllerBase
{
    // POST /api/eligibility/check
    [HttpPost("check")]
    public async Task<ActionResult<CheckEligibilityResponse>> CheckEligibility(CheckEligibilityRequest request)
    {
        // The controller only handles HTTP; the service applies the rule
        var result = await service.CheckEligibilityAsync(request);
        return Ok(result);
    }
}