using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Application.Common.Exceptions;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Application.Common.Mappings;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;
using UniversityMaintenance.Domain.Entities;
using UniversityMaintenance.Domain.Enums;

namespace UniversityMaintenance.Application.Services;

public class AssignmentService(IApplicationDbContext db, ICurrentUser currentUser)
    : IAssignmentService
{
    public async Task<AssignmentDto> AssignAsync(CreateAssignmentDto dto, CancellationToken ct = default)
    {
        var adminId = currentUser.UserId
            ?? throw new ForbiddenException("You must be signed in.");

        var request = await db.ServiceRequests
            .FirstOrDefaultAsync(r => r.Id == dto.ServiceRequestId, ct)
            ?? throw new NotFoundException("Service request not found.");

        var officer = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == dto.OfficerId, ct)
            ?? throw new NotFoundException("Officer not found.");

        if (officer.Role.Name != RoleNames.Officer)
            throw new BadRequestException("The selected user is not a maintenance officer.");

        var assignment = new Assignment
        {
            ServiceRequestId = request.Id,
            OfficerId = officer.Id,
            Officer = officer,
            ServiceRequest = request,
            AssignedByAdminId = adminId,
            AssignedAt = DateTime.UtcNow,
            Notes = dto.Notes
        };
        db.Assignments.Add(assignment);

        // Move the request forward and record it in the audit trail.
        if (request.Status is RequestStatus.Submitted or RequestStatus.Rejected)
        {
            var old = request.Status;
            request.Status = RequestStatus.Assigned;
            db.StatusUpdates.Add(new StatusUpdate
            {
                ServiceRequestId = request.Id,
                OldStatus = old,
                NewStatus = RequestStatus.Assigned,
                ChangedById = adminId,
                Comment = $"Assigned to {officer.FullName}."
            });
        }

        await db.SaveChangesAsync(ct);
        return assignment.ToDto();
    }
}
