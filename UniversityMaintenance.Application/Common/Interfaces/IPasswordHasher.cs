namespace UniversityMaintenance.Application.Common.Interfaces;

/// <summary>Hashes and verifies passwords. Implemented with ASP.NET Core's PasswordHasher.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string hash, string password);
}
