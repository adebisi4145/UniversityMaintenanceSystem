using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Application.Common.Exceptions;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services;
using UniversityMaintenance.Domain.Entities;
using UniversityMaintenance.Domain.Enums;
using UniversityMaintenance.Infrastructure.Persistence;

namespace UniversityMaintenance.Tests;

public class ServiceRequestServiceTests
{
    private static async Task<User> AddUserAsync(AppDbContext db, string role, string email)
    {
        var roleId = await TestSupport.RoleIdAsync(db, role);
        var user = new User { FirstName = "T", LastName = role, Email = email, PasswordHash = "x", RoleId = roleId };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    private static async Task<Guid> CategoryIdAsync(AppDbContext db) =>
        await db.Categories.Select(c => c.Id).FirstAsync();

    [Fact]
    public async Task Create_SetsSubmitted_AndWritesAuditEntry()
    {
        var db = await TestSupport.SeededContextAsync();
        var student = await AddUserAsync(db, RoleNames.Student, "s@x.edu");
        var current = new FakeCurrentUser { UserId = student.Id, Role = RoleNames.Student };
        var service = new ServiceRequestService(db, current);

        var created = await service.CreateAsync(new CreateServiceRequestDto
        {
            Title = "Leaky tap",
            Description = "Drip drip",
            Location = "Room 1",
            Priority = Priority.High,
            CategoryId = await CategoryIdAsync(db)
        }, evidenceImagePath: null);

        Assert.Equal(RequestStatus.Submitted, created.Status);
        Assert.Equal(1, await db.StatusUpdates.CountAsync(s => s.ServiceRequestId == created.Id));
    }

    [Fact]
    public async Task GetPaged_Student_SeesOnlyOwnRequests()
    {
        var db = await TestSupport.SeededContextAsync();
        var catId = await CategoryIdAsync(db);
        var alice = await AddUserAsync(db, RoleNames.Student, "alice@x.edu");
        var bob = await AddUserAsync(db, RoleNames.Student, "bob@x.edu");

        db.ServiceRequests.Add(new ServiceRequest
        { Title = "A", Description = "d", Location = "l", CategoryId = catId, RequesterId = alice.Id });
        db.ServiceRequests.Add(new ServiceRequest
        { Title = "B", Description = "d", Location = "l", CategoryId = catId, RequesterId = bob.Id });
        await db.SaveChangesAsync();

        var service = new ServiceRequestService(db,
            new FakeCurrentUser { UserId = alice.Id, Role = RoleNames.Student });

        var page = await service.GetPagedAsync(new ServiceRequestQuery());

        Assert.Single(page.Items);
        Assert.Equal("A", page.Items[0].Title);
    }

    [Fact]
    public async Task UpdateStatus_ByUnrelatedStudent_Throws()
    {
        var db = await TestSupport.SeededContextAsync();
        var catId = await CategoryIdAsync(db);
        var owner = await AddUserAsync(db, RoleNames.Student, "owner@x.edu");
        var other = await AddUserAsync(db, RoleNames.Student, "other@x.edu");

        var req = new ServiceRequest
        { Title = "A", Description = "d", Location = "l", CategoryId = catId, RequesterId = owner.Id };
        db.ServiceRequests.Add(req);
        await db.SaveChangesAsync();

        var service = new ServiceRequestService(db,
            new FakeCurrentUser { UserId = other.Id, Role = RoleNames.Student });

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            service.UpdateStatusAsync(req.Id, new UpdateStatusDto { Status = RequestStatus.Completed }));
    }

    [Fact]
    public async Task UpdateStatus_ByAdmin_ChangesStatus_AndLogsHistory()
    {
        var db = await TestSupport.SeededContextAsync();
        var catId = await CategoryIdAsync(db);
        var student = await AddUserAsync(db, RoleNames.Student, "st@x.edu");
        var admin = await AddUserAsync(db, RoleNames.Admin, "ad@x.edu");

        var req = new ServiceRequest
        { Title = "A", Description = "d", Location = "l", CategoryId = catId, RequesterId = student.Id };
        db.ServiceRequests.Add(req);
        await db.SaveChangesAsync();

        var service = new ServiceRequestService(db,
            new FakeCurrentUser { UserId = admin.Id, Role = RoleNames.Admin });

        var updated = await service.UpdateStatusAsync(req.Id,
            new UpdateStatusDto { Status = RequestStatus.InProgress, Comment = "Working" });

        Assert.Equal(RequestStatus.InProgress, updated.Status);
        var history = await service.GetHistoryAsync(req.Id);
        Assert.Contains(history, h => h.NewStatus == RequestStatus.InProgress && h.Comment == "Working");
    }
}
