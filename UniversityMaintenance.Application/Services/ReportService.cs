using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;

namespace UniversityMaintenance.Application.Services;

public class ReportService(IApplicationDbContext db) : IReportService
{
    public async Task<ReportSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var totalRequests = await db.ServiceRequests.CountAsync(ct);
        var totalUsers = await db.Users.CountAsync(ct);

        var byStatus = await db.ServiceRequests
            .GroupBy(r => r.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var byCategory = await db.ServiceRequests
            .Include(r => r.Category)
            .GroupBy(r => r.Category.Name)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var byPriority = await db.ServiceRequests
            .GroupBy(r => r.Priority)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return new ReportSummaryDto(
            totalRequests,
            totalUsers,
            byStatus.ToDictionary(x => x.Key.ToString(), x => x.Count),
            byCategory.ToDictionary(x => x.Key, x => x.Count),
            byPriority.ToDictionary(x => x.Key.ToString(), x => x.Count));
    }
}
