
using CountriesApi.Data;
using CountriesApi.Dtos;
using CountriesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CountriesApi.Controllers;


[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{
    private readonly CountriesDbContext _context;

    public CountriesController(CountriesDbContext context)
    {
        _context = context;
    }

    // GET /api/countries?activeOnly=true  → list countries (optionally only active ones)
    [HttpGet]
    public async Task<ActionResult<List<CountryDto>>> GetAll([FromQuery] bool activeOnly = false)
    {
        var query = _context.Countries.AsQueryable();
        if (activeOnly)
            query = query.Where(c => c.IsActive);

        var countries = await query.OrderBy(c => c.Name).ToListAsync();
        return countries.Select(ToDto).ToList();
    }

    // GET /api/countries/GHA  → one country; this is the endpoint the Requests API calls
    [HttpGet("{code}")]
    public async Task<ActionResult<CountryDto>> GetByCode(string code)
    {
        var country = await _context.Countries.FindAsync(code.ToUpper());
        if (country is null) return NotFound();   // 404
        return ToDto(country);                    // 200
    }

    // POST /api/countries  → add a country
    [HttpPost]
    public async Task<ActionResult<CountryDto>> Create(CountryDto dto) 
    {
        var code = dto.Code.Trim().ToUpper();
        if (code.Length != 3)
            return BadRequest("Country code must be 3 letters.");          // 400
        if (await _context.Countries.AnyAsync(c => c.IsoCode == code))
            return Conflict($"Country {code} already exists.");           // 409

        var country = new Country { IsoCode = code, Name = dto.Name.Trim(), IsActive = dto.IsActive };
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByCode), new { code }, ToDto(country));   // 201
    }

    // PUT /api/countries/GHA  → update a country's name or active flag (the code can't change)
    [HttpPut("{code}")]
    public async Task<IActionResult> Update(string code, UpdateCountryDto dto)
    {
        var country = await _context.Countries.FindAsync(code.ToUpper());
        if (country is null) return NotFound();   // 404

        country.Name = dto.Name.Trim();
        country.IsActive = dto.IsActive;
        await _context.SaveChangesAsync();
        return NoContent();                       // 204
    }

    // Converts the database entity into the JSON shape we send out
    private static CountryDto ToDto(Country c) => new(c.IsoCode, c.Name, c.IsActive);
}