using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using UniversityMaintenance.Application.DTOs;

namespace UniversityMaintenance.Tests;

public class ApiIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private async Task<string> RegisterAndGetTokenAsync(string email)
    {
        var res = await _client.PostAsJsonAsync("/api/auth/register", new RegisterDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = "Pass@123"
        });
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<AuthResponseDto>(Json);
        return body!.Token;
    }

    [Fact]
    public async Task Register_Then_Login_Works()
    {
        var email = $"user{Guid.NewGuid():N}@x.edu";
        await RegisterAndGetTokenAsync(email);

        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto { Email = email, Password = "Pass@123" });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/register", new RegisterDto
        {
            FirstName = "Bad",
            LastName = "Email",
            Email = "not-an-email",
            Password = "Pass@123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var res = await _client.GetAsync("/api/service-requests");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Student_CannotAccess_AdminReports_Returns403()
    {
        var token = await RegisterAndGetTokenAsync($"stud{Guid.NewGuid():N}@x.edu");
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/reports/summary");
        req.Headers.Authorization = new("Bearer", token);

        var res = await _client.SendAsync(req);

        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task RequestList_ReturnsPagedShape()
    {
        var token = await RegisterAndGetTokenAsync($"pg{Guid.NewGuid():N}@x.edu");
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/service-requests?page=1&pageSize=5");
        req.Headers.Authorization = new("Bearer", token);

        var res = await _client.SendAsync(req);
        res.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        Assert.True(root.TryGetProperty("items", out _));
        Assert.True(root.TryGetProperty("totalCount", out _));
        Assert.True(root.TryGetProperty("totalPages", out _));
    }
}
