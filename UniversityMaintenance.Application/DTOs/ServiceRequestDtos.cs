using UniversityMaintenance.Domain.Enums;

namespace UniversityMaintenance.Application.DTOs;

/// <summary>Fields for creating a request. The evidence image is uploaded separately as a file.</summary>
public class CreateServiceRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public Guid CategoryId { get; set; }
}

/// <summary>Officer/Admin moves a request to a new status, optionally with a comment.</summary>
public class UpdateStatusDto
{
    public RequestStatus Status { get; set; }
    public string? Comment { get; set; }
}

/// <summary>Query string parameters for searching, filtering and paging the request list.</summary>
public class ServiceRequestQuery
{
    public string? Search { get; set; }
    public RequestStatus? Status { get; set; }
    public Guid? CategoryId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public record ServiceRequestDto(
    Guid Id,
    string Title,
    string Description,
    string Location,
    Priority Priority,
    RequestStatus Status,
    string? EvidenceImagePath,
    Guid CategoryId,
    string CategoryName,
    Guid RequesterId,
    string RequesterName,
    Guid? AssignedOfficerId,
    string? AssignedOfficerName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>One entry in a request's status history / audit trail.</summary>
public record StatusUpdateDto(
    Guid Id,
    RequestStatus OldStatus,
    RequestStatus NewStatus,
    Guid ChangedById,
    string ChangedByName,
    string? Comment,
    DateTime CreatedAt);
