namespace CountriesApi.Dtos;

// What the API sends out (and what POST accepts): { "code": "GHA", "name": "Ghana", "isActive": true }
public record CountryDto(string Code, string Name, bool IsActive);

// Used by PUT: no Code, because the code is the key and comes from the URL
public record UpdateCountryDto(string Name, bool IsActive);