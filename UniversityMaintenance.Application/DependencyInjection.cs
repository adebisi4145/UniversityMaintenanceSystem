using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UniversityMaintenance.Application.Services;
using UniversityMaintenance.Application.Services.Interfaces;

namespace UniversityMaintenance.Application;

/// <summary>Registers Application-layer services and validators into the DI container.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IServiceRequestService, ServiceRequestService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
