# E2E Testing

## What BDD and Cucumber mean

BDD means Behavior-Driven Development. Tests are written from the user's point of view and describe behavior with Given/When/Then steps.

Cucumber is the common format for BDD scenarios. It uses Gherkin syntax:

```gherkin
Given an authenticated admin user
When a new department is created
Then the response status is Created
And the department can be retrieved from the API
```

## Tool used

This project uses xUnit for executable API-level E2E tests and stores the matching Cucumber-style Gherkin scenarios in:

`HRAPI.Tests/E2E/Features`

The executable tests are in:

`HRAPI.Tests/E2E/ApiBddE2ETests.cs`

SpecFlow/Cucumber packages were not already installed in the repository. The simplest reliable setup for final presentation readiness was to keep the existing xUnit test stack and add BDD-style E2E tests that call the running API over HTTP.

## Mapping to user stories

| Feature file | Requirement / user story area | Acceptance criteria covered |
|---|---|---|
| `Authentication.feature` | FR-01 Authentication & Authorization | Valid admin login returns JWT; protected endpoint accepts token |
| `DepartmentManagement.feature` | FR-02 Department Management | Admin can create department; created department can be retrieved |
| `PositionManagement.feature` | FR-03 Position Management | Admin can create position for existing department; created position can be retrieved |
| `EmployeeManagement.feature` | FR-04 Employee Management | Admin can create employee with valid department and position; created employee can be retrieved |
| `LeaveRequestManagement.feature` | FR-05 Leave Management | End date before start date is rejected with Bad Request |
| `AttendanceManagement.feature` | FR-06 Attendance Management | Duplicate attendance for same employee and date is rejected with Bad Request |

## What must be running before tests

The backend API must be running in Development mode.

Default URL:

`http://localhost:5065`

Start command:

```powershell
dotnet run --project .\HRAPI\HRAPI.csproj --launch-profile http
```

If the API uses another URL, set `HRAPI_E2E_BASE_URL` before running tests.

```powershell
$env:HRAPI_E2E_BASE_URL = "http://localhost:5065"
```

The SQLite database should be initialized by the application startup and contain the seeded accounts:

| Email | Password | Role |
|---|---|---|
| `admin@hr.com` | `Admin123!` | Admin |
| `hr@hr.com` | `Hr123!` | HRManager |
| `teamlead@hr.com` | `Team123!` | TeamLead |
| `employee@hr.com` | `Emp123!` | Employee |

## How to run the tests

From the repository root:

```powershell
dotnet test .\HRAPI.Tests\HRAPI.Tests.csproj --filter FullyQualifiedName~HRAPI.Tests.E2E
```

Run all backend tests:

```powershell
dotnet test .\HRAPI.Tests\HRAPI.Tests.csproj
```

## Covered scenarios

1. Admin can log in successfully.
2. Admin can create and retrieve a department.
3. Admin can create a position for an existing department.
4. Admin can create an employee.
5. Leave request cannot have an end date before the start date.
6. Attendance cannot be duplicated for the same employee and date.

## Expected results

All six E2E scenarios should pass. Create scenarios should return `201 Created`, retrieval checks should return `200 OK`, and validation scenarios should return `400 Bad Request`.

The tests create unique department codes, position titles, employee numbers, and employee emails so they can be rerun without manually cleaning the database.

## How to use the results in the final presentation

Show this order before demonstrating the application:

1. Functional requirements from `Docs/requirements.md`.
2. Non-functional requirements from `Docs/requirements.md`.
3. User stories and acceptance criteria summary from `Docs/presentation-prep.md`.
4. BDD feature files from `HRAPI.Tests/E2E/Features`.
5. Terminal output from the E2E command showing all scenarios passed.
6. Swagger at `http://localhost:5065/swagger`.
7. Application demo.
