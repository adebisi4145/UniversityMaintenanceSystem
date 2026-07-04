# University Maintenance System

**Live demo:** [university-maintenance-system-z45k.vercel.app](https://university-maintenance-system-z45k.vercel.app) ·
**API/Swagger:** [university-maintenance-api.onrender.com/swagger](https://university-maintenance-api.onrender.com/swagger)

A full-stack web application that replaces manual maintenance complaints (phone, paper, WhatsApp,
office visits) with a digital service desk. Students and staff submit and track service requests;
maintenance officers work assigned jobs and update progress; administrators manage users, assign
tasks, monitor status, and view reports.

## Tech stack

| Layer | Technology |
|---|---|
| Frontend | React 19 + TypeScript + Vite, React Router, Axios, React Hook Form + Zod |
| Backend | .NET 10 Web API (Clean Architecture: Domain / Application / Infrastructure / API) |
| Database | PostgreSQL via Entity Framework Core (Npgsql), code-first migrations |
| Auth | JWT bearer tokens with role claims |
| Docs | Swagger / OpenAPI + Postman collection |
| Tests | xUnit (backend), Vitest + React Testing Library (frontend) |

## Advanced features implemented

1. **JWT authentication** — stateless bearer tokens with role claims.
2. **Role-based access control** — `Student` / `Officer` / `Admin`, enforced on every endpoint.
3. **File / image upload** — evidence photos attached to requests.
4. **Search, filter & pagination** — on the request and user lists.
5. **Swagger / OpenAPI docs** — interactive UI with a JWT *Authorize* button.
6. **Audit trail** — every status change is recorded as a per-request activity timeline.
7. **CSV export** — admin report export (bonus).

## Project structure

```
UniversityMaintenanceSystem.slnx
├─ UniversityMaintenance.Domain/          entities, enums, BaseEntity
├─ UniversityMaintenance.Application/      DTOs, service interfaces + logic, validators, abstractions
├─ UniversityMaintenance.Infrastructure/   EF Core DbContext, migrations, JWT/hashing/file services, seeding
├─ UniversityMaintenance.API/              controllers, auth, middleware, Swagger, Program.cs
├─ UniversityMaintenance.Web/              React + Vite + TypeScript SPA
├─ UniversityMaintenance.Tests/            xUnit unit + integration tests
├─ docs/                                   report, deployment guide, API spec, screenshots
└─ docker-compose.yml                      local PostgreSQL
```

## Quick start

```bash
docker compose up -d                 # PostgreSQL on :5432
cd UniversityMaintenance.API && dotnet run        # API + Swagger on :5071
cd UniversityMaintenance.Web && npm install && npm run dev   # SPA on :5173
```

Default admin: `admin@university.edu` / `Admin@123`.

## Tests

```bash
dotnet test                                  # backend: 13 tests
cd UniversityMaintenance.Web && npm test      # frontend: 7 tests
```

## Documentation

- [Technical report](docs/REPORT.md) — problem statement, design, testing, deployment.
- [Deployment guide](docs/DEPLOYMENT.md) — Neon + Render + Vercel.
- [OpenAPI spec](docs/api/openapi.json) · [Postman collection](docs/api/UniversityMaintenance.postman_collection.json)
- [Interface screenshots](docs/screenshots/)
