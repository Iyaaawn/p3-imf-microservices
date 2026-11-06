using Microsoft.AspNetCore.Mvc;
using RequestsApi.Data;
using RequestsApi.Models;
using RequestsApi.Models.Dtos;
using Microsoft.EntityFrameworkCore; //
using Polly;
using RequestsApi.Clients;


namespace RequestsApi.Controllers;

[ApiController]
[Route("api/requests")]
public class RequestsController : ControllerBase
{

    private readonly RequestsDbContext _context;
    private readonly CountriesClient _countriesClient;
    private readonly EligibilityClient _eligibilityClient;
    public RequestsController(RequestsDbContext context,CountriesClient countriesClient, EligibilityClient eligibilityClient)
    {
        _context = context;
        _countriesClient = countriesClient;
        _eligibilityClient = eligibilityClient;
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var request = await _context.FundingRequests.FindAsync(id);
        if (request is null) return NotFound();

        if (request.Status != RequestStatus.Submitted)
            return Conflict(new ProblemDetails
            {
                Title = "Invalid status change",
                Detail = $"Only Submitted requests can be approved. This one is {request.Status}."
            });

        request.Status = RequestStatus.Approved;
        request.DecidedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult> Reject(int id)
    {
        var request = await _context.FundingRequests.FindAsync(id);
        if (request is null) return NotFound();

        if (request.Status != RequestStatus.Submitted)
            return Conflict(new ProblemDetails
            {
                Title = "Invalid status change",
                Detail = $"Only Submitted requests can be rejected. This one is {request.Status}."
            });

        request.Status = RequestStatus.Rejected;
        request.DecidedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<FundingRequest>>> GetAll()
    {
        return await _context.FundingRequests.OrderByDescending(r => r.CreatedAt).ToListAsync();

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FundingRequest>> GetByID(int id)
    {
        var request = await _context.FundingRequests.FindAsync(id);

        if (request == null)
        {
            return NotFound(); //Http Status Code: 404
        }

        return request;
    }
        [HttpPost]
    public async Task<IActionResult> Create(CreateFundingRequestDto dto)
    {
        if (dto.Amount <= 0)
            return Problem(detail: "Amount must be above 0", statusCode: 400);

        // 1. Does the country exist and is it active?
        CountryInfo? country;
        try { country = await _countriesClient.GetAsync(dto.CountryCode); }
        catch (Exception ex) when (ex is HttpRequestException or ExecutionRejectedException)
        { return Problem(detail: "Country service unavailable", statusCode: 503); }

        if (country is null) return Problem(detail: "Unknown country", statusCode: 400);
        if (!country.IsActive) return Problem(detail: "Country is not active", statusCode: 400);

        var request = new FundingRequest
        {
            CountryCode = country.Code,
            CountryName = country.Name,     // snapshot
            Fund = dto.Fund,
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow
        };

        // 2. Is the amount allowed?
        try
        {
            var result = await _eligibilityClient.CheckAsync(request);
            if (!result.Eligible)
            {
                // 422 as ProblemDetails; nothing is saved
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Not eligible",
                    Detail = result.Reason
                };
                problem.Extensions["maxAllowed"] = result.MaxAllowed;
                return UnprocessableEntity(problem);
            }

            request.Status = RequestStatus.Submitted;
            request.EligibilityReason = result.Reason;
        }
        catch (Exception ex) when (ex is HttpRequestException or ExecutionRejectedException)
        {
            request.Status = RequestStatus.PendingEligibility;   // Eligibility down: save now, recheck later
        }

        _context.FundingRequests.Add(request);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByID), new { id = request.Id }, request);
    }

    [HttpPost("{id:int}/recheck")]
    public async Task<IActionResult> Recheck(int id)
    {
        var request = await _context.FundingRequests.FindAsync(id);
        if (request is null) return NotFound();

        if (request.Status != RequestStatus.PendingEligibility)
            return Conflict(new ProblemDetails
            {
                Title = "Invalid status change",
                Detail = $"Only PendingEligibility requests can be rechecked. This one is {request.Status}."
            });

        try
        {
            var result = await _eligibilityClient.CheckAsync(request);
            request.EligibilityReason = result.Reason;
            if (result.Eligible)
                request.Status = RequestStatus.Submitted;
            else
            {
                request.Status = RequestStatus.Rejected;
                request.DecidedAt = DateTime.UtcNow;
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or ExecutionRejectedException)
        { return Problem(detail: "Eligibility service unavailable", statusCode: 503); }

        await _context.SaveChangesAsync();
        return Ok(request);
    }
    
    
}