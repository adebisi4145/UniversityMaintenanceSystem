using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the EF Core context so Application services depend on the
/// persistence shape, not on the Infrastructure implementation.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<RequestCategory> Categories { get; }
    DbSet<ServiceRequest> ServiceRequests { get; }
    DbSet<Assignment> Assignments { get; }
    DbSet<StatusUpdate> StatusUpdates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
