using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UniversityMaintenance.API.Common;
using UniversityMaintenance.API.Filters;
using UniversityMaintenance.API.Middleware;
using UniversityMaintenance.Application;
using UniversityMaintenance.Application.Common.Interfaces;
using UniversityMaintenance.Infrastructure;
using UniversityMaintenance.Infrastructure.Persistence;
using UniversityMaintenance.Infrastructure.Security;
using UniversityMaintenance.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Honor the PORT env var injected by cloud hosts (Render, Railway, Heroku).
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ---- Configuration objects ----
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

// ---- Services ----
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Resolve the physical uploads folder (served statically at /uploads).
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
Directory.CreateDirectory(uploadsPath);
builder.Services.PostConfigure<FileStorageOptions>(o => o.RootPath = uploadsPath);

// ---- Authentication / Authorization ----
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// ---- CORS ----
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod()));

// ---- Swagger with JWT support ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "University Maintenance System API",
        Version = "v1",
        Description = "REST API for submitting, assigning and tracking maintenance service requests."
    });

    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token (without the 'Bearer' prefix).",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
});

var app = builder.Build();

// ---- Migrate + seed the database on startup ----
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var adminEmail = builder.Configuration["Seed:AdminEmail"] ?? "admin@university.edu";
    var adminPassword = builder.Configuration["Seed:AdminPassword"] ?? "Admin@123";
    await DbSeeder.SeedAsync(db, hasher, adminEmail, adminPassword);
}

// ---- HTTP pipeline ----
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "University Maintenance API v1"));

// Serve uploaded evidence images at /uploads via an explicit provider (robust even when
// wwwroot does not exist at host-config time).
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposed so integration tests can reference the entry point via WebApplicationFactory.
public partial class Program;
