using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Domain.Entities;
using UniversityMaintenance.Infrastructure.Persistence;
using UniversityMaintenance.Infrastructure.Security;

namespace UniversityMaintenance.Tests;

/// <summary>Shared helpers for building an isolated in-memory context and real security services.</summary>
public static class TestSupport
{
    public static AppDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    public static async Task<AppDbContext> SeededContextAsync()
    {
        var db = NewContext();
        db.Roles.AddRange(
            new Role { Name = RoleNames.Student },
            new Role { Name = RoleNames.Officer },
            new Role { Name = RoleNames.Admin });
        db.Categories.Add(new RequestCategory { Name = "Electricity", Description = "Power" });
        await db.SaveChangesAsync();
        return db;
    }

    public static IPasswordHasher PasswordHasher() => new PasswordHasherService();

    public static IJwtTokenService JwtService() =>
        new JwtTokenService(Options.Create(new JwtSettings
        {
            Key = "test-signing-key-that-is-definitely-long-enough-1234567890",
            Issuer = "test",
            Audience = "test",
            ExpiryMinutes = 60
        }));

    public static Task<Guid> RoleIdAsync(AppDbContext db, string name) =>
        db.Roles.Where(r => r.Name == name).Select(r => r.Id).FirstAsync();
}

/// <summary>Test double for the authenticated-user accessor.</summary>
public class FakeCurrentUser : ICurrentUser
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool IsInRole(string role) => Role == role;
}
