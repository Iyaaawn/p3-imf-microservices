namespace EligibilityApi.Models;
public class CountryQuota
{
    public string CountryCode { get; set; } = "";
    public decimal QuotaSdrMillions { get; set; }
}