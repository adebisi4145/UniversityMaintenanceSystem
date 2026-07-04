namespace UniversityMaintenance.Application.DTOs;

public record UserDto(
    Guid Id, string FirstName, string LastName, string FullName, string Email, string Role, DateTime CreatedAt);

/// <summary>Admin-only creation of Officer/Admin accounts.</summary>
public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
