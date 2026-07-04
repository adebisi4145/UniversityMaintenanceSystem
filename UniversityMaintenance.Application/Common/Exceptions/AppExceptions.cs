namespace UniversityMaintenance.Application.Common.Exceptions;

/// <summary>Base class for expected, mappable application errors.</summary>
public abstract class AppException(string message) : Exception(message);

/// <summary>Requested resource does not exist. Maps to HTTP 404.</summary>
public sealed class NotFoundException(string message) : AppException(message);

/// <summary>Caller is authenticated but not allowed to perform the action. Maps to HTTP 403.</summary>
public sealed class ForbiddenException(string message) : AppException(message);

/// <summary>Request conflicts with current state (e.g. duplicate email). Maps to HTTP 409.</summary>
public sealed class ConflictException(string message) : AppException(message);

/// <summary>Bad input that is not covered by model validation. Maps to HTTP 400.</summary>
public sealed class BadRequestException(string message) : AppException(message);
