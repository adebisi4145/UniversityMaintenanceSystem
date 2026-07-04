# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first for restore-layer caching.
COPY UniversityMaintenance.Domain/UniversityMaintenance.Domain.csproj UniversityMaintenance.Domain/
COPY UniversityMaintenance.Application/UniversityMaintenance.Application.csproj UniversityMaintenance.Application/
COPY UniversityMaintenance.Infrastructure/UniversityMaintenance.Infrastructure.csproj UniversityMaintenance.Infrastructure/
COPY UniversityMaintenance.API/UniversityMaintenance.API.csproj UniversityMaintenance.API/
RUN dotnet restore UniversityMaintenance.API/UniversityMaintenance.API.csproj

# Copy the rest and publish.
COPY . .
RUN dotnet publish UniversityMaintenance.API/UniversityMaintenance.API.csproj -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# Render/Railway inject PORT at runtime; Program.cs binds to it.
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "UniversityMaintenance.API.dll"]
