using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Application.Common.Models;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;

namespace UniversityMaintenance.API.Controllers;

[ApiController]
[Authorize]
[Route("api/service-requests")]
public class ServiceRequestsController(
    IServiceRequestService requests,
    IFileStorageService fileStorage) : ControllerBase
{
    /// <summary>Paged, searchable, filterable list scoped to the caller's role.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ServiceRequestDto>>> Get(
        [FromQuery] ServiceRequestQuery query, CancellationToken ct)
        => Ok(await requests.GetPagedAsync(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceRequestDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await requests.GetByIdAsync(id, ct));

    /// <summary>Status history / audit trail for a request.</summary>
    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<StatusUpdateDto>>> GetHistory(Guid id, CancellationToken ct)
        => Ok(await requests.GetHistoryAsync(id, ct));

    /// <summary>Submit a new service request with an optional evidence image (multipart form).</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ServiceRequestDto>> Create(
        [FromForm] CreateServiceRequestDto dto, IFormFile? evidence, CancellationToken ct)
    {
        string? evidencePath = null;
        if (evidence is { Length: > 0 })
        {
            await using var stream = evidence.OpenReadStream();
            evidencePath = await fileStorage.SaveAsync(
                stream, evidence.FileName, evidence.ContentType, evidence.Length, ct);
        }

        var created = await requests.CreateAsync(dto, evidencePath, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update the status of a request (assigned officer or admin only).</summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ServiceRequestDto>> UpdateStatus(
        Guid id, UpdateStatusDto dto, CancellationToken ct)
        => Ok(await requests.UpdateStatusAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await requests.DeleteAsync(id, ct);
        return NoContent();
    }
}
