# Testing Evidence

Use this file as a checklist while collecting screenshots for the final presentation.

## Backend build

Command:

```powershell
dotnet build .\HRAPI\HRAPI.csproj
```

Expected result:

`Build succeeded` with 0 errors.

## Backend run

Command:

```powershell
dotnet run --project .\HRAPI\HRAPI.csproj --launch-profile http
```

Expected result:

The API starts on `http://localhost:5065`.

## Swagger

URL:

`http://localhost:5065/swagger`

Expected result:

Swagger UI opens and lists the API endpoints.

## E2E test command

Run this while the backend is already running:

```powershell
dotnet test .\HRAPI.Tests\HRAPI.Tests.csproj --filter FullyQualifiedName~HRAPI.Tests.E2E
```

Expected result:

All BDD E2E tests pass.

## Passed E2E scenario summary

| Feature | Scenario | Expected result |
|---|---|---|
| Authentication | Admin can log in successfully | JWT returned; protected endpoint accessible |
| Department management | Admin can create and retrieve a department | `201 Created`; `200 OK` on retrieve |
| Position management | Admin can create a position for an existing department | `201 Created`; `200 OK` on retrieve |
| Employee management | Admin can create an employee | `201 Created`; `200 OK` on retrieve |
| Leave request management | End date before start date is rejected | `400 Bad Request` |
| Attendance management | Duplicate attendance is rejected | First request `201 Created`; second request `400 Bad Request` |

## Unit test command

The project already has xUnit unit/controller tests in `HRAPI.Tests`.

Command:

```powershell
dotnet test .\HRAPI.Tests\HRAPI.Tests.csproj --filter "FullyQualifiedName!~HRAPI.Tests.E2E"
```

Expected result:

Existing non-E2E tests pass.

## Frontend build

Command:

```powershell
cd .\HRFrontend
npm run build
```

Expected result:

The Vite/TypeScript production build completes successfully.

## Screenshots and evidence to collect

Collect these screenshots for the final presentation:

1. `dotnet build` success.
2. Backend running on `http://localhost:5065`.
3. Swagger UI open at `/swagger`.
4. E2E test command with all six BDD scenarios passing.
5. Existing unit tests passing.
6. Frontend build passing.
7. Login screen and a successful authenticated page.
8. Departments, Positions, Employees, Leave, and Attendance pages.

## Latest local verification

Verified on June 12, 2026:

| Check | Command | Result |
|---|---|---|
| Backend build | `dotnet build .\HRAPI\HRAPI.csproj` | Passed, 0 warnings, 0 errors |
| Test project build | `dotnet build .\HRAPI.Tests\HRAPI.Tests.csproj` | Passed, 0 warnings, 0 errors |
| Swagger startup check | `GET http://localhost:5065/swagger/index.html` | Returned HTTP 200 while backend was running |
| BDD E2E tests | `dotnet test .\HRAPI.Tests\HRAPI.Tests.csproj --filter FullyQualifiedName~HRAPI.Tests.E2E --no-build` | Passed: 6, Failed: 0 |
| Existing non-E2E tests | `dotnet test .\HRAPI.Tests\HRAPI.Tests.csproj --filter "FullyQualifiedName!~HRAPI.Tests.E2E" --no-build` | Passed: 177, Failed: 0 |
| Frontend build | `npm.cmd run build` from `HRFrontend` | Passed; Vite reported a large chunk warning |

Note: In the Codex sandbox, commands that restore NuGet or npm packages required network approval. For normal local development, run the documented commands directly.
