using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("api/[controller]")]
public class AssignmentsController(IAssignmentService assignments) : ControllerBase
{
    /// <summary>Assign a service request to a maintenance officer (admin only).</summary>
    [HttpPost]
    public async Task<ActionResult<AssignmentDto>> Assign(CreateAssignmentDto dto, CancellationToken ct)
        => Ok(await assignments.AssignAsync(dto, ct));
}
