namespace UniversityMaintenance.Application.Common.Interfaces;

/// <summary>Exposes the authenticated user's identity, read from the HTTP context.</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }

    string? Email { get; }

    string? Role { get; }

    bool IsInRole(string role);
}
