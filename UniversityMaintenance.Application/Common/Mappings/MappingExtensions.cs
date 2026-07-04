using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Application.Common.Mappings;

/// <summary>Manual entity → DTO projections (kept dependency-free, no AutoMapper).</summary>
public static class MappingExtensions
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.FirstName, user.LastName, user.FullName, user.Email,
            user.Role?.Name ?? string.Empty, user.CreatedAt);

    public static CategoryDto ToDto(this RequestCategory c) =>
        new(c.Id, c.Name, c.Description);

    public static ServiceRequestDto ToDto(this ServiceRequest r)
    {
        // The most recent assignment identifies the currently responsible officer.
        var current = r.Assignments
            .OrderByDescending(a => a.AssignedAt)
            .FirstOrDefault();

        return new ServiceRequestDto(
            r.Id,
            r.Title,
            r.Description,
            r.Location,
            r.Priority,
            r.Status,
            r.EvidenceImagePath,
            r.CategoryId,
            r.Category?.Name ?? string.Empty,
            r.RequesterId,
            r.Requester?.FullName ?? string.Empty,
            current?.OfficerId,
            current?.Officer?.FullName,
            r.CreatedAt,
            r.UpdatedAt);
    }

    public static StatusUpdateDto ToDto(this StatusUpdate s) =>
        new(s.Id, s.OldStatus, s.NewStatus, s.ChangedById, s.ChangedBy?.FullName ?? string.Empty,
            s.Comment, s.CreatedAt);

    public static AssignmentDto ToDto(this Assignment a) =>
        new(a.Id, a.ServiceRequestId, a.ServiceRequest?.Title ?? string.Empty, a.OfficerId,
            a.Officer?.FullName ?? string.Empty, a.AssignedByAdminId, a.AssignedAt, a.Notes);
}
