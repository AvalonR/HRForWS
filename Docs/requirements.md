# HR System — Requirements Document

> **Project:** Human Resources Management System  
> **API:** .NET 10 Web API, RESTful (Richardson Maturity Model Level 3)  
> **Frontend:** Angular 19, MUI X DataGrid, Vite  
> **Database:** SQLite via Entity Framework Core  
> **Auth:** JWT Bearer + ASP.NET Core Identity  
> **Tests:** xUnit + Moq + FluentAssertions (177 unit tests), SpecFlow (E2E)  

---

## 1. Functional Requirements

### FR-01: Authentication & Authorization

| ID | Requirement | Source |
|---|---|---|
| FR-01.1 | User logs in via `POST /api/auth/login` with email + password; API returns a signed JWT containing user ID, email, and roles | GitHub Issue — Auth |
| FR-01.2 | Invalid credentials return 401 | Auth Issue AC |
| FR-01.3 | Four roles exist: **Admin**, **HRManager**, **TeamLead**, **Employee** | Seed data |
| FR-01.4 | `[Authorize]` and `[Authorize(Roles="...")]` applied to all controllers; unauthenticated = 401, unauthorized = 403 | Auth Issue AC |
| FR-01.5 | JWT configured in `appsettings.json` (key, issuer, audience, expiry) | Auth Issue |
| FR-01.6 | `AppDbContext` inherits from `IdentityDbContext<AppUser>`; `AppUser` extends `IdentityUser` with nullable `EmployeeId` FK | Auth Issue |
| FR-01.7 | Frontend login page at `/login`; on success JWT stored, user redirected to `/`; Logout functionality present | Auth Issue AC |
| FR-01.8 | `AuthContext` provides user, token, `isAuthenticated`, roles to entire app; Axios interceptor adds `Authorization: Bearer <token>`; 401 responses trigger automatic logout | Auth Issue AC |
| FR-01.9 | `ProtectedRoute` component redirects unauthenticated users to `/login`; Navbar items conditionally shown/hidden based on roles | Auth Issue AC |

### FR-02: Department Management

| ID | Requirement | Source |
|---|---|---|
| FR-02.1 | List all departments (GET `/api/departments`) with pagination | GitHub Issue — Departments |
| FR-02.2 | View a single department by ID (GET `/api/departments/{id}`) | Departments Issue |
| FR-02.3 | Create a department (POST `/api/departments`) with name, code, description, parent | Departments Issue |
| FR-02.4 | Update a department (PUT `/api/departments/{id}`) | Departments Issue |
| FR-02.5 | Delete a department (DELETE `/api/departments/{id}`) — blocked if employees exist | Departments Issue |
| FR-02.6 | Code must be unique; no self-referencing parent | Departments Issue |
| FR-02.7 | API returns 404 if department does not exist | Departments Issue AC |
| FR-02.8 | Frontend: DataGrid displays Code, Name, Description, Parent Department, Status; loading spinner, error state with retry, empty state message; "Add Department" button; Edit/Delete action buttons on each row; pagination (10/25/50 per page) | Departments Frontend Issue |

### FR-03: Position Management

| ID | Requirement | Source |
|---|---|---|
| FR-03.1 | List all positions (GET `/api/positions`) with department name | GitHub Issue — Positions |
| FR-03.2 | View / Create / Update / Delete positions | Positions Issue |
| FR-03.3 | Validates department exists; prevents duplicate title within same department | Positions Issue AC |
| FR-03.4 | Min salary ≤ Max salary | Positions Issue |
| FR-03.5 | API returns 404 if position does not exist | Positions Issue AC |
| FR-03.6 | Endpoints appear in Swagger; project builds successfully | Positions Issue AC |

### FR-04: Employee Management

| ID | Requirement | Source |
|---|---|---|
| FR-04.1 | List all employees (GET `/api/employees`) with full details (department, position, manager) | GitHub Issue — Employees |
| FR-04.2 | View / Create / Update / Delete employees | Employees Issue |
| FR-04.3 | Employee number and email must be unique | Employees Issue AC |
| FR-04.4 | Validates department, position, and manager exist | Employees Issue AC |
| FR-04.5 | Non-admin users should not see the delete option | Frontend Issue |
| FR-04.6 | Frontend: form fields — employee number, name, email, phone, hire date, department, position, manager; basic email format check before API call | Employee Form Issue |
| FR-04.7 | API returns 400 for duplicate employee number/email, invalid department, invalid position | Employees Issue AC |
| FR-04.8 | API returns 404 if employee does not exist | Employees Issue AC |

### FR-05: Leave Management

| ID | Requirement | Source |
|---|---|---|
| FR-05.1 | Two tabs: Leave Requests + Leave Types | Frontend Issue |
| FR-05.2 | CRUD for leave types (name, days allowed, paid/unpaid) | GitHub Issue — Leave Types |
| FR-05.3 | CRUD for leave requests (employee, leave type, start date, end date, status, reason) | GitHub Issue — Leave Requests |
| FR-05.4 | Admin/HR/TeamLead can see all requests; Employees see only their own | Frontend Issue |
| FR-05.5 | Approve / Reject actions on pending leave requests | Frontend Issue |
| FR-05.6 | New leave request has **Pending** status by default | Leave Request AC |
| FR-05.7 | API validates employee exists, leave type exists; prevents end date before start date | Leave Request AC |
| FR-05.8 | Date picker defaults to today (local time) | Frontend Issue |
| FR-05.9 | API returns 404 if leave request/type does not exist | Leave Issue AC |
| FR-05.10 | GET `/api/leavetypes`, GET `/api/leavetypes/{id}`, POST, PUT, DELETE — duplicate name returns 400, invalid daysAllowed returns 400 | Leave Types Swagger test |

### FR-06: Attendance Management

| ID | Requirement | Source |
|---|---|---|
| FR-06.1 | Record check-in / check-out with status | Frontend Issue |
| FR-06.2 | List attendance records with employee name and date | Frontend Issue |
| FR-06.3 | CRUD via `/api/attendances` endpoints | API Scope |

### FR-07: Payroll & Salary

| ID | Requirement | Source |
|---|---|---|
| FR-07.1 | Manage payroll records (base salary, overtime, bonuses, deductions, net pay) | Frontend Issue |
| FR-07.2 | Track salary history with effective dates and change reason | Frontend Issue |
| FR-07.3 | Manage deductions per employee | Frontend Issue |

### FR-08: Deductions Management

| ID | Requirement | Source |
|---|---|---|
| FR-08.1 | CRUD for deductions (type, amount, description) per payroll record | DeductionsController |
| FR-08.2 | Deduction amount must be greater than zero | DeductionService |
| FR-08.3 | Validates payroll record exists | DeductionService |
| FR-08.4 | Full HATEOAS envelopes returned on all actions | DeductionsController |
| FR-08.5 | **Frontend not yet implemented** — API-only; no route, page, or service in Angular app | Codebase audit |

### FR-09: Performance Reviews

| ID | Requirement | Source |
|---|---|---|
| FR-09.1 | Create performance reviews (rating, strengths, goals, next review date) | Frontend Issue |
| FR-09.2 | List and update reviews | Frontend Issue |
| FR-09.3 | Statuses: Draft, Pending, Completed, Cancelled | ReviewStatus enum |
| FR-09.4 | Two FKs to Employee (employee + reviewer) with restrict delete | AppDbContext |

### FR-10: HATEOAS (Richardson Maturity Model Level 3)

| ID | Requirement | Source |
|---|---|---|
| FR-10.1 | Every GET response includes a `_links` array with at least a `self` link pointing to the current resource | HATEOAS User Story |
| FR-10.2 | Collection responses wrap items in an `items` field and include collection-level links (`self`, `create`) | HATEOAS User Story AC |
| FR-10.3 | Single resource responses wrap data in a `data` field and include action links (`self`, `update`, `delete`) appropriate to the caller's role | HATEOAS User Story AC |
| FR-10.4 | Create responses (201) include the created resource's `data` + `_links` + a `Location` header | HATEOAS User Story AC |
| FR-10.5 | Update/Delete responses (204) return no body | HATEOAS User Story AC |
| FR-10.6 | HATEOAS built using anonymous types inside controllers (no `Resource<T>` wrapper classes) keeping DTOs unchanged | Implementation |

### FR-11: Output Caching

| ID | Requirement | Source |
|---|---|---|
| FR-10.1 | Output caching returns `Cache-Control: public, max-age=60` headers on Departments, Positions, and Leave Types endpoints | HATEOAS User Story AC |
| FR-10.2 | Cache invalidation — creating, updating, or deleting a cached entity immediately evicts the relevant cache entries so the next read returns fresh data | HATEOAS User Story AC |

### FR-12: Frontend UI (Cross-Cutting)

| ID | Requirement | Source |
|---|---|---|
| FR-12.1 | MUI X DataGrid for all list pages with pagination (10/25/50 per page) | Multiple Issues |
| FR-12.2 | Dialogs for create/edit forms | Multiple Issues |
| FR-12.3 | Delete confirmation prompt before every delete | Multiple Issues |
| FR-12.4 | Success/error snackbar after every action | Multiple Issues |
| FR-12.5 | Double-submit guard on all save buttons (`if (saving) return`) | Frontend Issues |
| FR-12.6 | Loading spinners during data fetch | Multiple Issues |
| FR-12.7 | Empty state message when no records exist | Multiple Issues |
| FR-12.8 | Error state with retry button if API call fails | Multiple Issues |
| FR-12.9 | Forms stay open on error so user can fix and retry | Frontend Issue |
| FR-12.10 | Frontend keeps working with HATEOAS — one-line Axios interceptor unwraps `items`/`data`/`_links` envelope transparently; no changes to any page or service file | HATEOAS User Story AC |

### FR-13: Seed Data

| ID | Requirement | Source |
|---|---|---|
| FR-13.1 | App ships with pre-loaded data — 6 departments, 6 positions, 6 employees, 4 user accounts, 3 leave requests, 6 attendance records | Frontend Issue / Seed |
| FR-13.2 | Test accounts: `admin@hr.com` (Admin), `hr@hr.com` (HRManager), `teamlead@hr.com` (TeamLead), `employee@hr.com` (Employee) | Seed Data Issue |
| FR-13.3 | App works immediately after cloning without manual DB setup | Seed Data Issue |
| FR-13.4 | 5 leave types seeded via `AppDbContext.OnModelCreating`: Annual (20d paid), Sick (10d paid), Personal (5d unpaid), Maternity (90d paid), Paternity (10d paid) | AppDbContext |

### FR-14: Recruitment & Applicant Tracking — Planned (Database Only)

| ID | Requirement | Status |
|---|---|---|
| FR-14.1 | **Job Posting** CRUD (title, description, position, employment type, posting/closing dates, status) | DB model built, no API |
| FR-14.2 | **Applicant** management (first/last name, email, phone, resume file path) | DB model built, no API |
| FR-14.3 | **Application** tracking (links job posting → applicant, status workflow: Submitted → Reviewed → Shortlisted → Rejected → Accepted) | DB model built, no API |
| FR-14.4 | **Interview** scheduling (links application → interviewer, type: Phone/Video/InPerson/Technical/HR, date, rating, notes) | DB model built, no API |
| FR-14.5 | Employment types: FullTime, PartTime, Contract, Internship, Temporary | EmploymentType enum |
| FR-14.6 | Job posting statuses: Draft, Open, Closed, Filled, Cancelled | JobPostingStatus enum |
| FR-14.7 | Applicant email is unique; application status stored as string in DB | AppDbContext config |

### FR-15: Service Layer Architecture

| ID | Requirement | Source |
|---|---|---|
| FR-15.1 | Business logic extracted from controllers into dedicated service classes (one per entity) | SERVICE_LAYER_PLAN.md |
| FR-15.2 | Service interface pattern: `I{Entity}Service` → `{Entity}Service` | Codebase |
| FR-15.3 | Shared `ServiceResult` / `ServiceResult<T>` pattern for all service returns (Succeeded / NotFound / ErrorMessage) | ServiceResult.cs |
| FR-15.4 | Controllers are thin — inject services, handle HTTP concerns only | Codebase |
| FR-15.5 | `IAuthService` wraps `UserManager<AppUser>` + `SignInManager<AppUser>` + JWT generation | AuthService.cs |
| FR-15.6 | All services registered via `AddScoped` in `Program.cs` | Program.cs |

### FR-16: Shared UI Components & Error Handling

| ID | Requirement | Source |
|---|---|---|
| FR-16.1 | Reusable `DeleteConfirmDialog` component used across all entity pages (shared from departments module) | Codebase |
| FR-16.2 | `getErrorMessage(err, fallback)` utility safely unwraps RFC 7807 Problem Details from API error responses | errorUtils.ts |
| FR-16.3 | Error handling distinguishes 401 (wrong credentials), server errors, network errors, and unexpected errors on login | LoginPage.tsx |
| FR-16.4 | `HomePage` shows welcome card with user email and roles; `pages/dashboard/` directory exists ready for future KPI dashboard | Codebase |

---

## 2. Non-Functional Requirements

| ID | Requirement | Details |
|---|---|---|
| NFR-01 | **Tech Stack** | .NET 10 Web API, Angular 19, SQLite, MUI X DataGrid, Axios |
| NFR-02 | **Authentication** | JWT Bearer tokens via ASP.NET Core Identity; `Microsoft.AspNetCore.Identity.EntityFrameworkCore` + `Microsoft.AspNetCore.Authentication.JwtBearer` |
| NFR-03 | **Database** | SQLite via Entity Framework Core with code-first migrations; `AppDbContext` inherits from `IdentityDbContext<AppUser>` |
| NFR-04 | **Test Coverage** | Minimum 177 unit tests passing (xUnit + Moq + FluentAssertions); all tests remain green after changes |
| NFR-05 | **API Documentation** | Swagger UI at `/swagger` via `Swashbuckle.AspNetCore`; `AddSwaggerGen`, `UseSwagger`, `UseSwaggerUI` configured in `Program.cs` |
| NFR-06 | **API Maturity** | Richardson Maturity Model Level 3 (HATEOAS) — all responses include hypermedia `_links` for API discoverability |
| NFR-07 | **Output Caching** | Server-side output caching (60s) on lookup endpoints (Departments, Positions, Leave Types); cache eviction on writes |
| NFR-08 | **Error Responses** | API returns Problem Details (RFC 7807) for errors; frontend `getErrorMessage()` utility for safe extraction |
| NFR-09 | **Code Style** | C# PascalCase, TypeScript camelCase, consistent naming conventions; documented in `Docs/git-workflow.md` |
| NFR-10 | **Frontend Build** | Vite + TypeScript, zero build warnings |
| NFR-11 | **CORS** | Frontend at `localhost:5173`, API at `localhost:5065`; CORS policy allows origin, methods, headers |
| NFR-12 | **Project Structure** | GitHub branches per feature (`feature/*`, `fix/*`); PRs reviewed before merge to `develop`; Project board tracks progress |
| NFR-13 | **CI/Quality** | `dotnet build` — clean (0 warnings, 0 errors); `dotnet test` — all passing; `npm run build` — clean |

---

## 3. Architecture Overview

```
┌─────────────────┐       ┌──────────────────────┐       ┌──────────┐
│   Angular 19     │──────▶│   .NET 10 Web API    │──────▶│  SQLite  │
│   (localhost:5173)│      │   (localhost:5065)    │       │   DB     │
│                  │◀──────│                      │◀──────│          │
│  Axios + JWT     │       │  JWT Auth + Output    │       └──────────┘
│  MUI X DataGrid  │       │  Cache + HATEOAS      │
└─────────────────┘       └──────────────────────┘
        │                          │
        └─────── Service Layer ────┘
        IAuthService, DepartmentService, etc.
```

### API Controllers (11 total)

| Controller | Auth | Output Cache | HATEOAS |
|---|---|---|---|
| `AuthController` | Public | No | No |
| `DepartmentsController` | Admin, HRManager | Yes (60s) | Yes |
| `PositionsController` | Admin, HRManager | Yes (60s) | Yes |
| `LeaveTypesController` | Admin, HRManager, TeamLead | Yes (60s) | Yes |
| `EmployeesController` | Admin, HRManager | No | Yes |
| `LeaveRequestsController` | All roles | No | Yes |
| `AttendancesController` | All roles | No | Yes |
| `PayrollRecordsController` | Admin, HRManager | No | Yes |
| `DeductionsController` | Admin, HRManager | No | Yes |
| `SalaryHistoriesController` | Admin, HRManager, Employee (own) | No | Yes |
| `PerformanceReviewsController` | Admin, HRManager, TeamLead | No | Yes |

### Base Class Hierarchy

```
ControllerBase
  └── ApiControllerBase        ← Shared: EvictCacheAsync(), Links(), ToActionResult()
       ├── AuthController
       ├── DepartmentsController
       ├── PositionsController
       ├── EmployeesController
       ├── LeaveTypesController
       ├── LeaveRequestsController
       ├── AttendancesController
       ├── PayrollRecordsController
       ├── DeductionsController
       ├── SalaryHistoriesController
       └── PerformanceReviewsController
```

---

## 4. API Endpoints Summary

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Login, returns JWT |
| GET | `/api/departments` | Admin, HRManager | List departments |
| GET | `/api/departments/{id}` | Admin, HRManager | Get department |
| POST | `/api/departments` | Admin, HRManager | Create department |
| PUT | `/api/departments/{id}` | Admin, HRManager | Update department |
| DELETE | `/api/departments/{id}` | Admin, HRManager | Delete department |
| GET | `/api/positions` | Admin, HRManager | List positions |
| GET | `/api/positions/{id}` | Admin, HRManager | Get position |
| POST | `/api/positions` | Admin, HRManager | Create position |
| PUT | `/api/positions/{id}` | Admin, HRManager | Update position |
| DELETE | `/api/positions/{id}` | Admin, HRManager | Delete position |
| GET | `/api/employees` | Admin, HRManager | List employees |
| GET | `/api/employees/{id}` | Admin, HRManager | Get employee |
| POST | `/api/employees` | Admin, HRManager | Create employee |
| PUT | `/api/employees/{id}` | Admin, HRManager | Update employee |
| DELETE | `/api/employees/{id}` | Admin | Delete employee |
| GET | `/api/leavetypes` | Admin, HRManager, TeamLead | List leave types |
| GET | `/api/leavetypes/{id}` | Admin, HRManager, TeamLead | Get leave type |
| POST | `/api/leavetypes` | Admin, HRManager | Create leave type |
| PUT | `/api/leavetypes/{id}` | Admin, HRManager | Update leave type |
| DELETE | `/api/leavetypes/{id}` | Admin, HRManager | Delete leave type |
| GET | `/api/leaverequests` | All roles | List leave requests |
| GET | `/api/leaverequests/{id}` | All roles | Get leave request |
| POST | `/api/leaverequests` | All roles | Create leave request |
| PUT | `/api/leaverequests/{id}` | All roles | Update leave request |
| DELETE | `/api/leaverequests/{id}` | All roles | Delete leave request |
| GET | `/api/attendances` | All roles | List attendance records |
| GET | `/api/attendances/{id}` | All roles | Get attendance record |
| POST | `/api/attendances` | All roles | Create attendance record |
| PUT | `/api/attendances/{id}` | All roles | Update attendance record |
| DELETE | `/api/attendances/{id}` | All roles | Delete attendance record |
| GET | `/api/payrollrecords` | Admin, HRManager | List payroll records |
| GET | `/api/payrollrecords/{id}` | Admin, HRManager | Get payroll record |
| POST | `/api/payrollrecords` | Admin, HRManager | Create payroll record |
| PUT | `/api/payrollrecords/{id}` | Admin, HRManager | Update payroll record |
| DELETE | `/api/payrollrecords/{id}` | Admin, HRManager | Delete payroll record |
| GET | `/api/deductions` | Admin, HRManager | List deductions |
| GET | `/api/deductions/{id}` | Admin, HRManager | Get deduction |
| POST | `/api/deductions` | Admin, HRManager | Create deduction |
| PUT | `/api/deductions/{id}` | Admin, HRManager | Update deduction |
| DELETE | `/api/deductions/{id}` | Admin | Delete deduction |
| GET | `/api/salaryhistories` | Admin, HRManager, Employee (own) | List salary histories |
| GET | `/api/salaryhistories/{id}` | Admin, HRManager, Employee (own) | Get salary history |
| POST | `/api/salaryhistories` | Admin, HRManager | Create salary history |
| PUT | `/api/salaryhistories/{id}` | Admin, HRManager | Update salary history |
| DELETE | `/api/salaryhistories/{id}` | Admin, HRManager | Delete salary history |
| GET | `/api/performancereviews` | Admin, HRManager, TeamLead | List performance reviews |
| GET | `/api/performancereviews/{id}` | Admin, HRManager, TeamLead | Get performance review |
| POST | `/api/performancereviews` | Admin, HRManager, TeamLead | Create performance review |
| PUT | `/api/performancereviews/{id}` | Admin, HRManager, TeamLead | Update performance review |
| DELETE | `/api/performancereviews/{id}` | Admin, HRManager, TeamLead | Delete performance review |

---

## 5. Seed Data Accounts

| Email | Password | Role |
|---|---|---|
| `admin@hr.com` | `Admin123!` | Admin |
| `hr@hr.com` | `Hr123!` | HRManager |
| `teamlead@hr.com` | `Team123!` | TeamLead |
| `employee@hr.com` | `Emp123!` | Employee |

---

## 6. Database Entity Model

### Version 1 (Implemented — API + Frontend)

| Entity | Key Relationships |
|---|---|
| **Department** | Parent → self-referencing; 1:N → Positions, Employees |
| **Position** | N:1 → Department; 1:N → Employees |
| **Employee** | N:1 → Department; N:1 → Position; N:1 → Employee (Manager); 1:N → LeaveRequests, Attendance |
| **LeaveType** | 1:N → LeaveRequests |
| **LeaveRequest** | N:1 → Employee; N:1 → LeaveType; N:1 → Employee (ReviewedBy) |
| **Attendance** | N:1 → Employee |
| **PayrollRecord** | N:1 → Employee |
| **Deduction** | N:1 → PayrollRecord |
| **SalaryHistory** | N:1 → Employee |
| **PerformanceReview** | N:1 → Employee; N:1 → Employee (Reviewer) |
| **AppUser** | Extends IdentityUser; N:1 → Employee (nullable) |

### Version 2 (Planned — Database Models Only, No API Yet)

| Entity | Key Relationships |
|---|---|
| **JobPosting** | N:1 → Position; EmploymentType + JobPostingStatus enums |
| **Applicant** | Email unique; ResumeFilePath optional |
| **Application** | N:1 → JobPosting; N:1 → Applicant; ApplicationStatus enum |
| **Interview** | N:1 → Application; N:1 → Employee (Interviewer); InterviewType enum |

---

## 7. Frontend Route Map

### Implemented Routes

| Route | Component | Auth | Roles |
|---|---|---|---|
| `/login` | `LoginPage` | Public | All |
| `/` | `HomePage` | Protected | All |
| `/departments` | `DepartmentsList` | Protected | Admin, HRManager |
| `/positions` | `PositionsList` | Protected | Admin, HRManager |
| `/employees` | `EmployeesList` | Protected | Admin, HRManager |
| `/leave` | `LeavePage` (tabs: Requests + Types) | Protected | All |
| `/attendance` | `AttendanceList` | Protected | All |
| `/payroll` | `PayrollList` | Protected | Admin, HRManager |
| `/salary-history` | `SalaryHistoryList` | Protected | Admin, HRManager |
| `/performance-reviews` | `PerformanceReviewsList` | Protected | Admin, HRManager, TeamLead |

### Future / Planned Routes

| Route | Component | Status |
|---|---|---|
| `/deductions` | Not yet built — API exists (`DeductionsController`), no frontend | API done, UI pending |
| `/dashboard` | Empty directory ready for KPI landing page | Placeholder only |
| `/applicants` | Future recruitment module (DB models exist) | Not started |
| `/job-postings` | Future recruitment module (DB models exist) | Not started |
| `/applications` | Future recruitment module (DB models exist) | Not started |
| `/interviews` | Future recruitment module (DB models exist) | Not started |

---

## 8. Glossary

| Term | Definition |
|---|---|
| **HATEOAS** | Hypermedia as the Engine of Application State — API responses include links to related actions |
| **Output Caching** | Server-side response caching to reduce load for repeated identical requests |
| **JWT** | JSON Web Token — stateless authentication token containing user claims |
| **Identity** | ASP.NET Core Identity framework for user management and role-based auth |
| **MUI X DataGrid** | Material UI's advanced data table component with sorting, filtering, pagination |
| **RFC 7807** | Problem Details — standardized error response format for HTTP APIs |
| **SpecFlow** | .NET implementation of Cucumber BDD framework for integration/E2E tests |
| **Richardson Maturity Model** | 4-level model for REST API maturity (Level 0-3); Level 3 = HATEOAS |
| **DTO** | Data Transfer Object — object that carries data between processes |
| **CORS** | Cross-Origin Resource Sharing — browser security mechanism for cross-origin requests |
| **ServiceResult** | Shared return type pattern for service-layer operations (Succeeded / NotFound / ErrorMessage + optional Data) |
| **Problem Details** | RFC 7807 standard error format used by ASP.NET Core (`application/problem+json`) |
| **Seed Data** | Pre-loaded database records that allow the application to function immediately after first run |
| **Anonymous Type Envelope** | HATEOAS wrapping via `new { items, _links }` / `new { data, _links }` in controllers without modifying DTOs |

---

*Document generated from closed GitHub issues, PRs, and codebase analysis. Last updated: June 2026.*
