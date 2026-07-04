using UniversityMaintenance.Application.DTOs;

namespace UniversityMaintenance.Application.Services.Interfaces;

public interface IReportService
{
    Task<ReportSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}
