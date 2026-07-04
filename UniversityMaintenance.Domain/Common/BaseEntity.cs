namespace UniversityMaintenance.Domain.Common;

/// <summary>
/// Base class for all persisted entities. Holds the primary key and audit
/// timestamps so no entity has to repeat them. <see cref="UpdatedAt"/> is
/// stamped centrally in the DbContext on every change.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
