namespace UniversityMaintenance.Application.DTOs;

public record CategoryDto(Guid Id, string Name, string? Description);

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
