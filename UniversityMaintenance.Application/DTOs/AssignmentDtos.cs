namespace UniversityMaintenance.Application.DTOs;

/// <summary>Admin assigns a request to a maintenance officer.</summary>
public class CreateAssignmentDto
{
    public Guid ServiceRequestId { get; set; }
    public Guid OfficerId { get; set; }
    public string? Notes { get; set; }
}

public record AssignmentDto(
    Guid Id,
    Guid ServiceRequestId,
    string ServiceRequestTitle,
    Guid OfficerId,
    string OfficerName,
    Guid AssignedByAdminId,
    DateTime AssignedAt,
    string? Notes);
