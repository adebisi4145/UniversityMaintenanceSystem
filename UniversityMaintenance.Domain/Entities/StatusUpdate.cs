using UniversityMaintenance.Domain.Common;
using UniversityMaintenance.Domain.Enums;

namespace UniversityMaintenance.Domain.Entities;

/// <summary>
/// An entry in a request's activity/audit trail: records who changed the status,
/// from what to what, and any comment.
/// </summary>
public class StatusUpdate : BaseEntity
{
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; } = null!;

    public RequestStatus OldStatus { get; set; }

    public RequestStatus NewStatus { get; set; }

    /// <summary>The user (officer/admin) who made the change.</summary>
    public Guid ChangedById { get; set; }
    public User ChangedBy { get; set; } = null!;

    public string? Comment { get; set; }
}
