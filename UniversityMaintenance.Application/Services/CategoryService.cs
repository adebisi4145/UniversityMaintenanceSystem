using Microsoft.EntityFrameworkCore;
using UniversityMaintenance.Application.Common.Exceptions;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Application.Common.Mappings;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services.Interfaces;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Application.Services;

public class CategoryService(IApplicationDbContext db) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await db.Categories.OrderBy(c => c.Name).ToListAsync(ct);
        return categories.Select(c => c.ToDto()).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var name = dto.Name.Trim();
        if (await db.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct))
            throw new ConflictException("A category with this name already exists.");

        var category = new RequestCategory { Name = name, Description = dto.Description?.Trim() };
        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);
        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, CreateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException("Category not found.");

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim();
        await db.SaveChangesAsync(ct);
        return category.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException("Category not found.");

        if (await db.ServiceRequests.AnyAsync(r => r.CategoryId == id, ct))
            throw new ConflictException("Cannot delete a category that has service requests.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
    }
}
