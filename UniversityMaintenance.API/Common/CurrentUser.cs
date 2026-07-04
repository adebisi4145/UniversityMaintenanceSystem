using System.Security.Claims;
using UniversityMaintenance.Application.Common.Interfaces;

namespace UniversityMaintenance.API.Common;

/// <summary>Reads the authenticated user's identity from the current HTTP request's claims.</summary>
public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public Guid? UserId =>
        Guid.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public string? Email => Principal?.FindFirstValue(ClaimTypes.Email);

    public string? Role => Principal?.FindFirstValue(ClaimTypes.Role);

    public bool IsInRole(string role) => Principal?.IsInRole(role) ?? false;
}
