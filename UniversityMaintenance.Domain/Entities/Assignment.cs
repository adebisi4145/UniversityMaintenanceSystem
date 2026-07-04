using UniversityMaintenance.Domain.Common;

namespace UniversityMaintenance.Domain.Entities;

/// <summary>Links a service request to the maintenance officer responsible for it.</summary>
public class Assignment : BaseEntity
{
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; } = null!;

    /// <summary>The maintenance officer the request is assigned to.</summary>
    public Guid OfficerId { get; set; }
    public User Officer { get; set; } = null!;

    /// <summary>The admin who made the assignment.</summary>
    public Guid AssignedByAdminId { get; set; }

    public DateTime AssignedAt { get; set; }

    public string? Notes { get; set; }
}
