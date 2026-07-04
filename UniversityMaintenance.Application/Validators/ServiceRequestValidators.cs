using FluentValidation;
using UniversityMaintenance.Application.DTOs;

namespace UniversityMaintenance.Application.Validators;

public class CreateServiceRequestDtoValidator : AbstractValidator<CreateServiceRequestDto>
{
    public CreateServiceRequestDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public class UpdateStatusDtoValidator : AbstractValidator<UpdateStatusDto>
{
    public UpdateStatusDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}

public class CreateAssignmentDtoValidator : AbstractValidator<CreateAssignmentDto>
{
    public CreateAssignmentDtoValidator()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty();
        RuleFor(x => x.OfficerId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
