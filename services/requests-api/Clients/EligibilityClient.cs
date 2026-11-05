using RequestsApi.Models;

namespace RequestsApi.Clients;

// What the Eligibility API sends back (Requests API's own copy of the shape)
public record EligibilityResult(bool Eligible, decimal MaxAllowed, string Reason);

public class EligibilityClient(HttpClient http)
{
    public async Task<EligibilityResult?> CheckAsync(FundingRequest fr)
    {
        // Build the JSON body from the funding request's fields
        var body = new { fr.CountryCode, Fund = fr.Fund.ToString(), fr.Amount };

        var response = await http.PostAsJsonAsync("api/eligibility/check", body);
        response.EnsureSuccessStatusCode();   // throws if Eligibility returned an error
        return await response.Content.ReadFromJsonAsync<EligibilityResult>();
    }
}