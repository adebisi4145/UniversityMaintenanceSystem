# University Maintenance System — Technical Report

## 1. Introduction and problem statement

The university receives maintenance complaints and service requests through fragmented manual
channels — phone calls, paper forms, WhatsApp messages, and in-person office visits. This causes
delays, missing records, poor tracking, and a lack of accountability: there is no single place to see
who reported what, who is responsible, and what state a job is in.

The **University Maintenance System** is a full-stack web application that digitises this process.
Students and staff submit service requests (faulty electricity, damaged furniture, leaking pipes,
internet problems, classroom equipment, hostel complaints) and track them to completion. Maintenance
officers see the jobs assigned to them and update progress. Administrators manage users, assign work
to officers, monitor all requests, and generate reports.

## 2. System objectives

- Provide a single digital channel for submitting and tracking maintenance requests.
- Enforce clear roles and accountability (requester → admin → officer) with a full audit trail.
- Let officers update job progress and mark work complete.
- Give administrators oversight: user management, task assignment, status monitoring, and reporting.
- Secure the system with authentication, authorization, and safe password handling.
- Be documented, tested, and deployable online.

## 3. Requirement analysis

**Actors / roles**

| Role | Capabilities |
|---|---|
| Student / Staff | Register, log in, submit requests (with evidence photo), track own requests + history |
| Maintenance Officer | View assigned jobs, update status/progress, mark complete |
| Administrator | Manage users & categories, assign requests to officers, view all requests, view reports |

**Functional requirements** — registration/login; role dashboards; request submission form; request
tracking; admin request management; CRUD on requests; request assignment; status updates with history.

**Non-functional requirements** — JWT security & RBAC; validation and consistent error handling;
secure password hashing; responsive UI; API documentation; automated tests; online deployment.

**Advanced features implemented** (7, exceeding the required 4): JWT auth, role-based access control,
file/image upload, search + filter + pagination, Swagger/OpenAPI docs, audit trail/activity log, and
CSV export.

## 4. Frontend technologies used

- **React 19 + TypeScript**, built with **Vite**.
- **React Router** for role-aware, protected routing.
- **Axios** with interceptors that attach the JWT to every request and redirect to login on `401`.
- **React Hook Form + Zod** for typed forms with client-side validation.
- A hand-written CSS design system (sidebar layout, cards, tables, badges, timeline, toasts) that is
  responsive down to mobile widths.

Key structure: `src/api` (typed client + endpoints), `src/auth` (JWT context + `ProtectedRoute`),
`src/components` (Layout, StatusBadge, Pagination, Toast), `src/pages` (auth, dashboard, requests,
admin).

## 5. Backend technologies used

- **.NET 10 Web API** organised in **Clean Architecture** layers:
  - **Domain** — entities, enums, and a shared `BaseEntity` (`Id`, `CreatedAt`, `UpdatedAt`).
  - **Application** — DTOs, service interfaces + business logic, FluentValidation validators, and
    abstractions (`IApplicationDbContext`, `IPasswordHasher`, `IJwtTokenService`, `IFileStorageService`,
    `ICurrentUser`) so the core depends on interfaces, not infrastructure.
  - **Infrastructure** — EF Core `AppDbContext`, entity configurations, migrations, JWT/password/file
    services, and database seeding.
  - **API** — controllers, JWT authentication, RBAC, global exception-handling middleware, a
    FluentValidation action filter, Swagger, CORS, and startup wiring.
- **Security**: passwords hashed with ASP.NET Core's PBKDF2 `PasswordHasher`; JWTs signed with
  HMAC-SHA256 carrying the user id, email, and role; `[Authorize(Roles = ...)]` on protected endpoints.
- `UpdatedAt` is stamped centrally in `SaveChanges` via the EF `ChangeTracker`, so services never
  set audit timestamps by hand.

## 6. Database and relationships

**PostgreSQL** accessed through **EF Core (Npgsql)**, code-first with migrations.

Entities: `Role`, `User`, `RequestCategory`, `ServiceRequest`, `Assignment`, `StatusUpdate`
(all inheriting `BaseEntity`).

Relationship types (all **one-to-many**, giving a normalised relational schema):

| Parent | Child | Meaning |
|---|---|---|
| Role | User | a role has many users; a user has one role |
| RequestCategory | ServiceRequest | a category classifies many requests |
| User (requester) | ServiceRequest | a user submits many requests |
| ServiceRequest | Assignment | a request has assignment history (one active officer) |
| User (officer) | Assignment | an officer is assigned many requests |
| ServiceRequest | StatusUpdate | a request has many audit-trail entries |
| User (changed by) | StatusUpdate | a user authors many status changes |

Constraints: unique indexes on `User.Email`, `Role.Name`, `RequestCategory.Name`; enums stored as
readable strings; foreign keys use `Restrict` (users/categories) or `Cascade` (a request's assignments
and status updates) as appropriate.

## 7. API documentation

RESTful endpoints, documented interactively with **Swagger** (JWT *Authorize* button) and shipped as a
[Postman collection](api/UniversityMaintenance.postman_collection.json) and an exported
[OpenAPI spec](api/openapi.json).

| Method & path | Auth | Purpose |
|---|---|---|
| `POST /api/auth/register` | anon | Register a student/staff account |
| `POST /api/auth/login` | anon | Log in, receive a JWT |
| `GET /api/service-requests` | any | List (search, filter, paginate), role-scoped |
| `POST /api/service-requests` | any | Submit a request (multipart, optional image) |
| `GET /api/service-requests/{id}` | any | Request details |
| `GET /api/service-requests/{id}/history` | any | Status history / audit trail |
| `PATCH /api/service-requests/{id}/status` | officer/admin | Update status (logs history) |
| `DELETE /api/service-requests/{id}` | owner/admin | Delete a request |
| `POST /api/assignments` | admin | Assign a request to an officer |
| `GET/POST/PUT/DELETE /api/categories` | any / admin | Category management |
| `GET/POST/DELETE /api/users`, `GET /api/users/officers` | admin | User management |
| `GET /api/reports/summary` | admin | Aggregate counts for the dashboard |

Errors return a consistent JSON shape (`{ "error": "..." }`); validation failures return `400` with
per-field messages; unauthorized/forbidden return `401`/`403`.

## 8. Screenshots of major interfaces

Captured from the running application ([`docs/screenshots/`](screenshots/)):

| Interface | File |
|---|---|
| Login | `screenshots/01-login.png` |
| Admin dashboard | `screenshots/02-admin-dashboard.png` |
| All requests (search/filter/paginate) | `screenshots/03-admin-requests.png` |
| Reports (charts + CSV export) | `screenshots/04-admin-reports.png` |
| User management | `screenshots/05-admin-users.png` |
| Registration | `screenshots/07-register.png` |
| Student dashboard | `screenshots/08-student-dashboard.png` |
| Submit request form | `screenshots/09-submit-request.png` |
| Request detail + activity timeline | `screenshots/10-student-request-detail.png` |

## 9. Testing evidence

**Backend — xUnit (13 tests, all passing):**
- Unit tests (in-memory DbContext, real hashing/JWT): register creates a Student and normalises email;
  duplicate email rejected; wrong password rejected; login succeeds; request creation sets `Submitted`
  and writes an audit entry; students see only their own requests; a non-owner cannot change status;
  an admin status change updates state and logs history.
- Integration tests (`WebApplicationFactory`, full pipeline): register→login works; invalid email → `400`;
  no token → `401`; a student hitting admin reports → `403`; request list returns the paged shape.

**Frontend — Vitest + React Testing Library (7 tests, all passing):** status/priority badge labels;
pagination (disabled Prev on page 1, advances on Next, hidden when empty); login form shows a
validation error and does not call the API on invalid input, and calls the API on valid input.

**End-to-end** — a Playwright run drove the real UI against the live API + PostgreSQL through the full
lifecycle (admin login → dashboards → reports → users → register student → submit request → request
detail) with **zero console/network errors**; this also produced the screenshots above and caught a
real defect (an EF query ordering by a computed property) that in-memory tests could not.

```
dotnet test        → Passed! 13/13
npm test           → 7 passed (7)
```

## 10. Deployment information

**Live URLs:**

| Component | URL |
|---|---|
| Frontend (Vercel) | https://university-maintenance-system-z45k.vercel.app |
| API + Swagger (Render) | https://university-maintenance-api.onrender.com/swagger |
| Database | PostgreSQL on Neon (US East) |

Three-tier deployment (all free-tier capable): **PostgreSQL on Neon**, **API on Render** (Docker), and
the **React SPA on Vercel**. The deployed stack was verified end-to-end in a real browser (register,
login, submit, assign, status updates, audit trail) with zero errors. The repo ships a `Dockerfile`, a `render.yaml` blueprint, `vercel.json`
(SPA rewrites), and `docker-compose.yml` for local Postgres. The API reads its connection string, JWT
secret, and allowed CORS origin from environment variables, applies EF Core migrations, and seeds
roles, categories, and the default admin on startup. Full steps are in
[DEPLOYMENT.md](DEPLOYMENT.md).

## 11. Challenges encountered and solutions

- **Swagger vs Microsoft.OpenApi 2.x** — the newest Swashbuckle pulled an OpenAPI model API with
  breaking namespace changes. Pinned Swashbuckle to a stable line so the JWT *Authorize* setup works
  reliably.
- **`FullName` not translatable to SQL** — after switching users to `FirstName`/`LastName` with a
  computed `FullName`, EF could not translate `OrderBy(u => u.FullName)`. Caught by the end-to-end run
  against real PostgreSQL (in-memory tests evaluate it client-side); fixed by ordering on the mapped
  columns.
- **Integration tests and the DB provider** — `WebApplicationFactory` inherited the Npgsql provider,
  clashing with the in-memory provider. Solved by removing all `AppDbContext` service descriptors
  (including EF 10's `IDbContextOptionsConfiguration<T>`) and making the seeder use `EnsureCreated`
  for the in-memory provider.
- **Ephemeral upload storage** — local-disk uploads are wiped on free-tier redeploys. Kept the
  `IFileStorageService` abstraction so cloud blob storage can drop in without touching business logic.

## 12. Conclusion

The University Maintenance System delivers the full brief: role-based dashboards for students/staff,
officers, and administrators; request submission with evidence upload; tracking with a complete audit
trail; and admin management and reporting. It is built on a clean, layered .NET 10 backend with a
PostgreSQL/EF Core data layer and a typed React frontend, secured with JWT and RBAC, documented with
Swagger/Postman, covered by 20 automated tests plus an end-to-end browser run, and ready to deploy on
free cloud tiers. The architecture leaves clear extension points — cloud file storage, email/real-time
notifications, and richer reporting — for future work.
