# Deployment Guide

The system deploys as three pieces: a **PostgreSQL database**, the **.NET API**, and the **React frontend**.
The stack is free-tier friendly (Neon + Render + Vercel).

```
React SPA (Vercel/Netlify)  ──HTTPS──►  .NET API (Render/Railway)  ──►  PostgreSQL (Neon)
```

---

## 1. Database — Neon PostgreSQL (free)

1. Create a project at https://neon.tech and copy the connection string.
2. Convert it to the .NET/Npgsql format, e.g.:
   ```
   Host=ep-xxx.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=****;SSL Mode=Require;Trust Server Certificate=true
   ```
   The API applies EF Core migrations and seeds roles, categories, and the default admin automatically on first start.

## 2. API — Render (Docker)

The repo includes [`UniversityMaintenance.API/Dockerfile`](../UniversityMaintenance.API/Dockerfile) and a
[`render.yaml`](../render.yaml) blueprint.

1. Push the repo to GitHub.
2. On Render: **New → Blueprint**, point at the repo. It reads `render.yaml`.
3. Set these environment variables (Render dashboard → Environment):
   | Variable | Value |
   |---|---|
   | `ConnectionStrings__DefaultConnection` | the Neon connection string above |
   | `Jwt__Key` | a long random secret (≥ 32 chars) |
   | `Jwt__Issuer` | `UniversityMaintenance` |
   | `Jwt__Audience` | `UniversityMaintenanceClient` |
   | `Cors__AllowedOrigins__0` | the frontend URL, e.g. `https://your-app.vercel.app` |
   | `Seed__AdminEmail` / `Seed__AdminPassword` | initial admin credentials |
4. Deploy. The service listens on the `PORT` Render injects (handled in `Program.cs`).
   Health check: `/swagger/v1/swagger.json`.

> Railway works the same way (Docker deploy + the same env vars). Note `__` (double underscore)
> maps to nested config keys in .NET.

## 3. Frontend — Vercel (or Netlify)

1. On Vercel: **Add New → Project**, import the repo, set **Root Directory** to `UniversityMaintenance.Web`.
2. Add an environment variable:
   | Variable | Value |
   |---|---|
   | `VITE_API_BASE_URL` | the deployed API origin, e.g. `https://university-maintenance-api.onrender.com` |
3. Deploy. [`vercel.json`](../UniversityMaintenance.Web/vercel.json) rewrites all routes to `index.html`
   so client-side routing works. (Netlify equivalent: `public/_redirects`.)

## 4. Wire the two together

- Set the frontend's `VITE_API_BASE_URL` to the API URL and redeploy.
- Set the API's `Cors__AllowedOrigins__0` to the frontend URL and redeploy.
- Verify end-to-end: register → login → submit a request with an image → (admin) assign → (officer)
  update status → track the audit trail.

## Notes / limitations

- **Uploaded images** are written to local disk (`wwwroot/uploads`). On free tiers this storage is
  **ephemeral** (cleared on redeploy). Acceptable for a demo; the production upgrade path is cloud blob
  storage (S3 / Azure Blob) behind the existing `IFileStorageService` abstraction.
- Keep `Jwt__Key` and the admin password as secrets — never commit real values.

## Local development

```bash
# 1. Database
docker compose up -d                       # Postgres on localhost:5432

# 2. API  (from repo root)
cd UniversityMaintenance.API
dotnet run                                 # http://localhost:5071  (Swagger at /swagger)

# 3. Frontend
cd UniversityMaintenance.Web
npm install
npm run dev                                # http://localhost:5173
```

Default admin (from `appsettings.json`): `admin@university.edu` / `Admin@123`.
