using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Infrastructure.Persistence;

/// <summary>Seeds the baseline data every deployment needs: roles, categories, and a default admin.</summary>
public static class DbSeeder
{
    private static readonly (string Name, string Description)[] DefaultCategories =
    [
        ("Electricity", "Faulty electricity, power sockets, lighting"),
        ("Plumbing", "Leaking pipes, taps, drainage, toilets"),
        ("Furniture", "Damaged desks, chairs, beds, cupboards"),
        ("Internet", "Wi-Fi and network connectivity problems"),
        ("Equipment", "Classroom and lab equipment issues"),
        ("Hostel", "General hostel maintenance complaints")
    ];

    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHasher passwordHasher,
        string adminEmail,
        string adminPassword,
        CancellationToken ct = default)
    {
        // Apply migrations against a real relational database; fall back to EnsureCreated
        // for the in-memory provider used in tests.
        var isInMemory = db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        if (isInMemory)
            await db.Database.EnsureCreatedAsync(ct);
        else
            await db.Database.MigrateAsync(ct);

        // Roles
        foreach (var name in new[] { RoleNames.Student, RoleNames.Officer, RoleNames.Admin })
        {
            if (!await db.Roles.AnyAsync(r => r.Name == name, ct))
                db.Roles.Add(new Role { Name = name });
        }
        await db.SaveChangesAsync(ct);

        // Categories
        foreach (var (catName, description) in DefaultCategories)
        {
            if (!await db.Categories.AnyAsync(c => c.Name == catName, ct))
                db.Categories.Add(new RequestCategory { Name = catName, Description = description });
        }
        await db.SaveChangesAsync(ct);

        // Default admin account
        var email = adminEmail.Trim().ToLowerInvariant();
        if (!await db.Users.AnyAsync(u => u.Email == email, ct))
        {
            var adminRole = await db.Roles.FirstAsync(r => r.Name == RoleNames.Admin, ct);
            db.Users.Add(new User
            {
                FirstName = "System",
                LastName = "Administrator",
                Email = email,
                PasswordHash = passwordHasher.Hash(adminPassword),
                RoleId = adminRole.Id
            });
            await db.SaveChangesAsync(ct);
        }
    }
}
