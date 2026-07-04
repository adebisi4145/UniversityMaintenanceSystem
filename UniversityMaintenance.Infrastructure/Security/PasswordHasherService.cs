using Microsoft.AspNetCore.Identity;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Infrastructure.Security;

/// <summary>Wraps ASP.NET Core's PBKDF2-based <see cref="PasswordHasher{T}"/>.</summary>
public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new User(), password);

    public bool Verify(string hash, string password)
    {
        var result = _hasher.VerifyHashedPassword(new User(), hash, password);
        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
