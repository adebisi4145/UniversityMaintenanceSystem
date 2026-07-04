namespace UniversityMaintenance.Infrastructure.Security;

/// <summary>Bound from the "Jwt" configuration section.</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "UniversityMaintenance";
    public string Audience { get; set; } = "UniversityMaintenance";
    public int ExpiryMinutes { get; set; } = 120;
}
