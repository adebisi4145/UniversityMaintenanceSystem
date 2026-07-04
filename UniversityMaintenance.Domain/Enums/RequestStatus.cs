namespace UniversityMaintenance.Domain.Enums;

/// <summary>Lifecycle states a service request moves through.</summary>
public enum RequestStatus
{
    Submitted = 0,
    Assigned = 1,
    InProgress = 2,
    Completed = 3,
    Rejected = 4
}
