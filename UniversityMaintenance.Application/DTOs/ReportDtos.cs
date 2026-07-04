namespace UniversityMaintenance.Application.DTOs;

/// <summary>Aggregate counts for the admin dashboard and reports.</summary>
public record ReportSummaryDto(
    int TotalRequests,
    int TotalUsers,
    IReadOnlyDictionary<string, int> RequestsByStatus,
    IReadOnlyDictionary<string, int> RequestsByCategory,
    IReadOnlyDictionary<string, int> RequestsByPriority);
