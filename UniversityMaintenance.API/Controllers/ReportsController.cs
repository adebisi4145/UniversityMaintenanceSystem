using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("api/[controller]")]
public class ReportsController(IReportService reports) : ControllerBase
{
    /// <summary>Aggregate counts for the admin dashboard (admin only).</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ReportSummaryDto>> Summary(CancellationToken ct)
        => Ok(await reports.GetSummaryAsync(ct));
}
