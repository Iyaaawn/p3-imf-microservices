using Microsoft.AspNetCore.Mvc;
using RequestsApi.Data;
using RequestsApi.Models;
using Microsoft.EntityFrameworkCore; //


namespace RequestsApi.Controllers;

[ApiController]
[Route("api/requests")]
public class RequestsController : ControllerBase
{

    private readonly RequestsDbContext _context;
    public RequestsController(RequestsDbContext context)
    {
        _context = context;
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
    
    
}