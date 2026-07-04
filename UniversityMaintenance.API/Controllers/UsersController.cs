using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMaintenance.Application.Common.Models;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("api/[controller]")]
public class UsersController(IUserService users) : ControllerBase
{
    /// <summary>Paged, searchable list of all users (admin only).</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> Get(
        [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
        => Ok(await users.GetPagedAsync(search, page, pageSize, ct));

    /// <summary>List maintenance officers (for the assignment dropdown).</summary>
    [HttpGet("officers")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetOfficers(CancellationToken ct)
        => Ok(await users.GetOfficersAsync(ct));

    /// <summary>Create an Officer or Admin account (admin only).</summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto, CancellationToken ct)
        => Ok(await users.CreateAsync(dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await users.DeleteAsync(id, ct);
        return NoContent();
    }
}
