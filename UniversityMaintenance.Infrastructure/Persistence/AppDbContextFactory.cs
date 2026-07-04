using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UniversityMaintenance.Infrastructure.Persistence;

/// <summary>
/// Used only by the EF Core CLI (migrations/scaffolding) so it can build the context
/// without running the API host. The connection string here is never used to talk to a
/// live database — only to select the Npgsql provider.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DESIGN_TIME_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=university_maintenance;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
