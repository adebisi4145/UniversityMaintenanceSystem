namespace UniversityMaintenance.Application.DTOs;

/// <summary>Public self-registration. New accounts always get the Student role.</summary>
public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>Returned on successful login/registration.</summary>
public record AuthResponseDto(string Token, DateTime ExpiresAt, UserDto User);
