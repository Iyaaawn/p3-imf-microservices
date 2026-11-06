using System.Net;

namespace RequestsApi.Clients;

public record CountryInfo(string Code, string Name, bool IsActive);

public class CountriesClient(HttpClient http)
{
    // null = the country doesn't exist (404); throws if Countries is down
    public async Task<CountryInfo?> GetAsync(string code)
    {
        var response = await http.GetAsync($"api/countries/{code}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CountryInfo>();
    }
}