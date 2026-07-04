using UniversityMaintenance.Application.Common.Models;
using UniversityMaintenance.Application.DTOs;

namespace UniversityMaintenance.Application.Services.Interfaces;

public interface IServiceRequestService
{
    Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto, string? evidenceImagePath,
        CancellationToken ct = default);

    /// <summary>Paged list, scoped by the current user's role (own / assigned / all).</summary>
    Task<PagedResult<ServiceRequestDto>> GetPagedAsync(ServiceRequestQuery query, CancellationToken ct = default);

    Task<ServiceRequestDto> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<ServiceRequestDto> UpdateStatusAsync(Guid id, UpdateStatusDto dto, CancellationToken ct = default);

    Task<IReadOnlyList<StatusUpdateDto>> GetHistoryAsync(Guid id, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
