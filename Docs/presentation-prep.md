# Presentation Preparation

## Required presentation order

The teacher requirement says these items must be shown before the project demo:

1. Functional requirements.
2. Non-functional requirements.
3. User stories with acceptance criteria.
4. Results of automated End-to-End tests written according to user stories using BDD and Cucumber-style scenarios.
5. Project demonstration.

## Functional requirements summary

Core implemented features:

| Area | Summary |
|---|---|
| Authentication | Seeded users log in with email/password and receive JWT bearer tokens |
| Authorization | Role-based access for Admin, HRManager, TeamLead, and Employee |
| Departments | Create, read, update, delete, pagination-ready listing, HATEOAS links |
| Positions | Create positions under departments and validate department relationship |
| Employees | Manage employee records with unique employee number and email |
| Leave types | Manage leave type definitions such as Annual and Sick leave |
| Leave requests | Submit and validate leave requests; new requests start as Pending |
| Attendance | Record attendance and prevent duplicates for same employee/date |
| Payroll, salary, reviews | API/frontend support for HR management workflows |
| Swagger | Interactive API documentation available in Development mode |

Do not present JobPosting, Applicant, Application, or Interview as completed features. They are planned database-only recruitment models.

## Non-functional requirements summary

| Area | Summary |
|---|---|
| Technology | ASP.NET Core Web API, Angular/Vite frontend, SQLite, Entity Framework Core |
| Security | JWT bearer authentication and ASP.NET Core Identity |
| API quality | RESTful endpoints with HATEOAS response envelopes |
| Validation | Service layer validates duplicates, missing related records, invalid dates, and invalid ranges |
| Testability | xUnit unit tests plus API-level BDD E2E tests |
| Documentation | Requirements, Swagger verification, database model notes, E2E testing notes, evidence checklist |
| Local demo | Backend on `http://localhost:5065`; frontend on `http://localhost:5173` |

## User stories with acceptance criteria summary

### Authentication

As an admin, I want to log in with valid credentials so I can access protected HR endpoints.

Acceptance criteria:

- `POST /api/auth/login` returns a JWT for `admin@hr.com`.
- A protected endpoint accepts the token.
- Invalid or missing tokens cannot access protected endpoints.

### Department management

As an admin, I want to create and retrieve departments so the company structure can be managed.

Acceptance criteria:

- `POST /api/departments` creates a department.
- Duplicate/invalid data is rejected.
- `GET /api/departments/{id}` retrieves the created department.

### Position management

As an admin, I want to create positions for departments so employees can be assigned valid roles.

Acceptance criteria:

- A position must reference an existing department.
- `POST /api/positions` creates a position.
- `GET /api/positions/{id}` retrieves the created position.

### Employee management

As an admin, I want to create employees so HR records can be maintained.

Acceptance criteria:

- Employee number and email are unique.
- Department and position must exist.
- `POST /api/employees` creates an employee.
- `GET /api/employees/{id}` retrieves the employee.

### Leave request management

As an authenticated user, I want invalid leave request dates to be rejected so leave records stay correct.

Acceptance criteria:

- Leave request must reference an existing employee.
- Leave request must reference an existing leave type.
- End date before start date returns `400 Bad Request`.

### Attendance management

As an admin, I want duplicate attendance records blocked so attendance data is consistent.

Acceptance criteria:

- First attendance record for employee/date can be created.
- Second attendance record for the same employee/date returns `400 Bad Request`.

## Agile/Scrum workflow summary

Suggested speaking points:

- Work was split into feature areas: authentication, departments, positions, employees, leave, attendance, payroll/salary, and documentation.
- Each feature was implemented through small Git branches and pull requests into `develop`.
- Requirements and acceptance criteria were documented in `Docs/requirements.md`.
- Testing was added at service/controller level first, then BDD E2E tests were added to prove user-story behavior through the real API.

## GitHub branches, PRs, and project board summary

Suggested speaking points:

- `main` is protected as the stable branch.
- `develop` is the integration branch.
- Feature work is done on `feature/*` branches.
- This testing/documentation work uses `feature/e2e-testing-and-presentation-docs`.
- Pull requests should target `develop`, not `main`.
- Project board items can be grouped by To Do, In Progress, Review, and Done.

## BDD/Cucumber E2E test results

BDD scenarios are documented in:

`HRAPI.Tests/E2E/Features`

Executable tests are in:

`HRAPI.Tests/E2E/ApiBddE2ETests.cs`

Run command:

```powershell
dotnet test .\HRAPI.Tests\HRAPI.Tests.csproj --filter FullyQualifiedName~HRAPI.Tests.E2E
```

Expected result:

Six E2E scenarios pass:

1. Authentication: Admin can log in successfully.
2. Department management: Admin can create and retrieve a department.
3. Position management: Admin can create a position for an existing department.
4. Employee management: Admin can create an employee.
5. Leave request management: Invalid date range returns Bad Request.
6. Attendance management: Duplicate attendance returns Bad Request.

## Project demo order

1. Open `Docs/requirements.md` and summarize functional requirements.
2. Summarize non-functional requirements.
3. Show user stories and acceptance criteria in this document.
4. Show `.feature` files in `HRAPI.Tests/E2E/Features`.
5. Run or show screenshot of E2E tests passing.
6. Open Swagger at `http://localhost:5065/swagger`.
7. Show frontend login.
8. Log in as `admin@hr.com`.
9. Demonstrate Departments, Positions, Employees, Leave, and Attendance.
10. Mention that recruitment models are planned/database-only and not part of this final demo.

## Suggested speaking points

- "The project is an HR management system with authenticated role-based access."
- "The backend exposes REST endpoints with HATEOAS response links and Swagger documentation."
- "The frontend consumes the API using JWT bearer authentication."
- "The most important user stories are validated with automated BDD-style E2E tests."
- "The E2E tests use real HTTP calls against the running backend and create unique data so they can be rerun."
- "Only completed features are demonstrated; planned recruitment models are not presented as finished functionality."
