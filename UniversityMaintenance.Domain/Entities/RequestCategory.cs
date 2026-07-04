using UniversityMaintenance.Domain.Common;

namespace UniversityMaintenance.Domain.Entities;

/// <summary>A type of fault, e.g. Electricity, Plumbing, Furniture, Internet, Equipment, Hostel.</summary>
public class RequestCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
