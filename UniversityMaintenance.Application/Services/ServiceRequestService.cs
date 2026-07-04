using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Application.Common.Exceptions;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Application.Common.Mappings;
using UniversityMaintenance.Application.Common.Models;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;
using UniversityMaintenance.Domain.Entities;
using UniversityMaintenance.Domain.Enums;

namespace UniversityMaintenance.Application.Services;

public class ServiceRequestService(IApplicationDbContext db, ICurrentUser currentUser)
    : IServiceRequestService
{
    private Guid UserId => currentUser.UserId
        ?? throw new ForbiddenException("You must be signed in.");

    public async Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto,
        string? evidenceImagePath, CancellationToken ct = default)
    {
        var categoryExists = await db.Categories.AnyAsync(c => c.Id == dto.CategoryId, ct);
        if (!categoryExists)
            throw new BadRequestException("The selected category does not exist.");

        var request = new ServiceRequest
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Location = dto.Location.Trim(),
            Priority = dto.Priority,
            Status = RequestStatus.Submitted,
            EvidenceImagePath = evidenceImagePath,
            CategoryId = dto.CategoryId,
            RequesterId = UserId
        };

        db.ServiceRequests.Add(request);

        // Seed the audit trail with the initial submission.
        db.StatusUpdates.Add(new StatusUpdate
        {
            ServiceRequestId = request.Id,
            OldStatus = RequestStatus.Submitted,
            NewStatus = RequestStatus.Submitted,
            ChangedById = UserId,
            Comment = "Request submitted."
        });

        await db.SaveChangesAsync(ct);
        return await GetByIdAsync(request.Id, ct);
    }

    public async Task<PagedResult<ServiceRequestDto>> GetPagedAsync(ServiceRequestQuery query,
        CancellationToken ct = default)
    {
        var q = BaseQuery();

        // Role-based scoping: students see their own, officers see assigned, admins see all.
        if (currentUser.IsInRole(RoleNames.Student))
            q = q.Where(r => r.RequesterId == UserId);
        else if (currentUser.IsInRole(RoleNames.Officer))
            q = q.Where(r => r.Assignments.Any(a => a.OfficerId == UserId));

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(r =>
                r.Title.ToLower().Contains(term) ||
                r.Description.ToLower().Contains(term) ||
                r.Location.ToLower().Contains(term));
        }

        if (query.Status.HasValue)
            q = q.Where(r => r.Status == query.Status.Value);

        if (query.CategoryId.HasValue)
            q = q.Where(r => r.CategoryId == query.CategoryId.Value);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 10 : query.PageSize;

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<ServiceRequestDto>(
            items.Select(r => r.ToDto()).ToList(), page, pageSize, total);
    }

    public async Task<ServiceRequestDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var request = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new NotFoundException("Service request not found.");

        EnsureCanView(request);
        return request.ToDto();
    }

    public async Task<ServiceRequestDto> UpdateStatusAsync(Guid id, UpdateStatusDto dto,
        CancellationToken ct = default)
    {
        var request = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new NotFoundException("Service request not found.");

        // Only the assigned officer or an admin may change status.
        var isAssignedOfficer = currentUser.IsInRole(RoleNames.Officer)
            && request.Assignments.Any(a => a.OfficerId == UserId);
        if (!currentUser.IsInRole(RoleNames.Admin) && !isAssignedOfficer)
            throw new ForbiddenException("You are not allowed to update this request.");

        if (dto.Status == request.Status)
            throw new BadRequestException("The request is already in that status.");

        var old = request.Status;
        request.Status = dto.Status;

        db.StatusUpdates.Add(new StatusUpdate
        {
            ServiceRequestId = request.Id,
            OldStatus = old,
            NewStatus = dto.Status,
            ChangedById = UserId,
            Comment = dto.Comment
        });

        await db.SaveChangesAsync(ct);
        return await GetByIdAsync(request.Id, ct);
    }

    public async Task<IReadOnlyList<StatusUpdateDto>> GetHistoryAsync(Guid id,
        CancellationToken ct = default)
    {
        var request = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new NotFoundException("Service request not found.");

        EnsureCanView(request);

        var history = await db.StatusUpdates
            .Include(s => s.ChangedBy)
            .Where(s => s.ServiceRequestId == id)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync(ct);

        return history.Select(s => s.ToDto()).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var request = await db.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new NotFoundException("Service request not found.");

        // Admins may delete any request; a requester may delete their own while still Submitted.
        var isOwnerAndPending = request.RequesterId == UserId
            && request.Status == RequestStatus.Submitted;
        if (!currentUser.IsInRole(RoleNames.Admin) && !isOwnerAndPending)
            throw new ForbiddenException("You are not allowed to delete this request.");

        db.ServiceRequests.Remove(request);
        await db.SaveChangesAsync(ct);
    }

    private IQueryable<ServiceRequest> BaseQuery() =>
        db.ServiceRequests
            .Include(r => r.Category)
            .Include(r => r.Requester)
            .Include(r => r.Assignments).ThenInclude(a => a.Officer)
            .AsQueryable();

    private void EnsureCanView(ServiceRequest request)
    {
        if (currentUser.IsInRole(RoleNames.Admin))
            return;
        if (currentUser.IsInRole(RoleNames.Officer)
            && request.Assignments.Any(a => a.OfficerId == UserId))
            return;
        if (request.RequesterId == UserId)
            return;

        throw new ForbiddenException("You are not allowed to view this request.");
    }
}
