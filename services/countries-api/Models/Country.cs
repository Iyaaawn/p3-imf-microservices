namespace CountriesApi.Models;

public class Country
{
    public string IsoCode { get; set; } = "";   // "GHA": the primary key and what other services use
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
}