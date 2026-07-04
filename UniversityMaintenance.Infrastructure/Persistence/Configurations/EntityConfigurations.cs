using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMaintenance.Domain.Entities;

namespace UniversityMaintenance.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(r => r.Name).IsUnique();

        builder.HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(75);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(75);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        // FullName is derived from FirstName + LastName; do not persist it.
        builder.Ignore(u => u.FullName);
    }
}

public class RequestCategoryConfiguration : IEntityTypeConfiguration<RequestCategory>
{
    public void Configure(EntityTypeBuilder<RequestCategory> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.HasIndex(c => c.Name).IsUnique();
    }
}

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Description).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.Location).IsRequired().HasMaxLength(200);
        builder.Property(r => r.EvidenceImagePath).HasMaxLength(400);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Priority).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.ServiceRequests)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Requester)
            .WithMany(u => u.ServiceRequests)
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.Property(a => a.Notes).HasMaxLength(1000);

        builder.HasOne(a => a.ServiceRequest)
            .WithMany(r => r.Assignments)
            .HasForeignKey(a => a.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Officer)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.OfficerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StatusUpdateConfiguration : IEntityTypeConfiguration<StatusUpdate>
{
    public void Configure(EntityTypeBuilder<StatusUpdate> builder)
    {
        builder.Property(s => s.Comment).HasMaxLength(1000);
        builder.Property(s => s.OldStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.NewStatus).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(s => s.ServiceRequest)
            .WithMany(r => r.StatusUpdates)
            .HasForeignKey(s => s.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.ChangedBy)
            .WithMany()
            .HasForeignKey(s => s.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
