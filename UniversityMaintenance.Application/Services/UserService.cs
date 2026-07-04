using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Application.Common.Exceptions;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Application.Common.Mappings;
using UniversityMaintenance.Application.Common.Models;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Application.Services;

public class UserService(
    IApplicationDbContext db,
    IPasswordHasher passwordHasher,
    ICurrentUser currentUser) : IUserService
{
    private static readonly string[] AssignableRoles =
        [RoleNames.Student, RoleNames.Officer, RoleNames.Admin];

    public async Task<PagedResult<UserDto>> GetPagedAsync(string? search, int page, int pageSize,
        CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var q = db.Users.Include(u => u.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            q = q.Where(u => u.FullName.ToLower().Contains(term) || u.Email.ToLower().Contains(term));
        }

        var total = await q.CountAsync(ct);
        var users = await q
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<UserDto>(users.Select(u => u.ToDto()).ToList(), page, pageSize, total);
    }

    public async Task<IReadOnlyList<UserDto>> GetOfficersAsync(CancellationToken ct = default)
    {
        var officers = await db.Users
            .Include(u => u.Role)
            .Where(u => u.Role.Name == RoleNames.Officer)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(ct);
        return officers.Select(u => u.ToDto()).ToList();
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        if (!AssignableRoles.Contains(dto.Role))
            throw new BadRequestException("Invalid role.");

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            throw new ConflictException("An account with this email already exists.");

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role, ct)
            ?? throw new NotFoundException("Role not found.");

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(dto.Password),
            RoleId = role.Id,
            Role = role
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (currentUser.UserId == id)
            throw new BadRequestException("You cannot delete your own account.");

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException("User not found.");

        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}
