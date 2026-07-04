using UniversityMaintenance.Domain.Common;

namespace UniversityMaintenance.Domain.Entities;

/// <summary>A security role: Student/Staff, Officer, or Admin. One role has many users.</summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}

/// <summary>Canonical role names used across the app for RBAC.</summary>
public static class RoleNames
{
    public const string Student = "Student";
    public const string Officer = "Officer";
    public const string Admin = "Admin";
}
