using UniversityMaintenance.Application.Common.Models;
using UniversityMaintenance.Application.DTOs;

namespace UniversityMaintenance.Application.Services.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetPagedAsync(string? search, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> GetOfficersAsync(CancellationToken ct = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
