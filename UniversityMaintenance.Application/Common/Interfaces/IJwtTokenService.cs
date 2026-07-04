using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Application.Common.Interfaces;

/// <summary>Issues signed JWT bearer tokens carrying the user's id, email, and role.</summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
