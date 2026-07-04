using UniversityMaintenance.Domain.Common;
using UniversityMaintenance.Domain.Enums;

namespace UniversityMaintenance.Domain.Entities;

/// <summary>A maintenance complaint or service request raised by a student/staff member.</summary>
public class ServiceRequest : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public Priority Priority { get; set; } = Priority.Medium;

    public RequestStatus Status { get; set; } = RequestStatus.Submitted;

    /// <summary>Relative path to the uploaded evidence image, if any.</summary>
    public string? EvidenceImagePath { get; set; }

    public Guid CategoryId { get; set; }
    public RequestCategory Category { get; set; } = null!;

    /// <summary>The user who submitted the request.</summary>
    public Guid RequesterId { get; set; }
    public User Requester { get; set; } = null!;

    /// <summary>Assignment history for this request (typically one active).</summary>
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    /// <summary>Audit trail of every status change on this request.</summary>
    public ICollection<StatusUpdate> StatusUpdates { get; set; } = new List<StatusUpdate>();
}
