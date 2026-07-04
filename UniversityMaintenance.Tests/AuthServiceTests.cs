using UniversityMaintenance.Application.Common.Exceptions;
using UniversityMaintenance.Application.DTOs;
using UniversityMaintenance.Application.Services;

namespace UniversityMaintenance.Tests;

public class AuthServiceTests
{
    private static AuthService Build(Infrastructure.Persistence.AppDbContext db) =>
        new(db, TestSupport.PasswordHasher(), TestSupport.JwtService());

    [Fact]
    public async Task Register_CreatesStudent_AndReturnsToken()
    {
        var db = await TestSupport.SeededContextAsync();
        var service = Build(db);

        var result = await service.RegisterAsync(new RegisterDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "Jane@Student.edu",
            Password = "Pass@123"
        });

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal("Student", result.User.Role);
        Assert.Equal("jane@student.edu", result.User.Email); // normalized to lowercase
        Assert.Equal("Jane Doe", result.User.FullName);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Throws()
    {
        var db = await TestSupport.SeededContextAsync();
        var service = Build(db);
        var dto = new RegisterDto { FirstName = "A", LastName = "B", Email = "dup@x.edu", Password = "Pass@123" };

        await service.RegisterAsync(dto);

        await Assert.ThrowsAsync<ConflictException>(() => service.RegisterAsync(dto));
    }

    [Fact]
    public async Task Login_WrongPassword_Throws()
    {
        var db = await TestSupport.SeededContextAsync();
        var service = Build(db);
        await service.RegisterAsync(new RegisterDto
        { FirstName = "A", LastName = "B", Email = "user@x.edu", Password = "Correct@1" });

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.LoginAsync(new LoginDto { Email = "user@x.edu", Password = "Wrong@1" }));
    }

    [Fact]
    public async Task Login_CorrectPassword_Succeeds()
    {
        var db = await TestSupport.SeededContextAsync();
        var service = Build(db);
        await service.RegisterAsync(new RegisterDto
        { FirstName = "A", LastName = "B", Email = "ok@x.edu", Password = "Correct@1" });

        var result = await service.LoginAsync(new LoginDto { Email = "ok@x.edu", Password = "Correct@1" });

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal("ok@x.edu", result.User.Email);
    }
}
