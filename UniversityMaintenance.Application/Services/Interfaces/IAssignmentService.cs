using UniversityMaintenance.Application.DTOs;

namespace UniversityMaintenance.Application.Services.Interfaces;

public interface IAssignmentService
{
    Task<AssignmentDto> AssignAsync(CreateAssignmentDto dto, CancellationToken ct = default);
}
