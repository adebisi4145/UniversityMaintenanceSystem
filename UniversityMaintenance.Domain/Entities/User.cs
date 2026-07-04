using UniversityMaintenance.Domain.Common;

namespace UniversityMaintenance.Domain.Entities;

/// <summary>An account that can log in. Each user has exactly one role.</summary>
public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    /// <summary>Convenience display name; not stored (computed from first + last).</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    /// <summary>Requests this user has submitted.</summary>
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

    /// <summary>Assignments where this user is the maintenance officer.</summary>
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
