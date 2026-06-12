# Frontend Testing

## Scope

Frontend testing was performed for final presentation readiness. No frontend behavior, UI code, routes, services, or backend functionality was changed.

## Frontend structure inspected

Frontend path:

`HRFrontend`

Main route file:

`HRFrontend/src/App.tsx`

Implemented routes checked:

| Page | Route | Access |
|---|---|---|
| Login | `/login` | Public |
| Home / Dashboard | `/` | Authenticated |
| Departments | `/departments` | Admin, HRManager, TeamLead |
| Positions | `/positions` | Admin, HRManager, TeamLead |
| Employees | `/employees` | Admin, HRManager, TeamLead |
| Leave | `/leave` | Admin, HRManager, TeamLead, Employee |
| Attendance | `/attendance` | Admin, HRManager, TeamLead |
| Payroll | `/payroll` | Admin, HRManager |
| Salary History | `/salary-history` | Admin, HRManager, TeamLead |
| Performance Reviews | `/performance-reviews` | Admin, HRManager, TeamLead |

## Dependency install

`node_modules` already existed from the previous verification run.

If dependencies are missing, use:

```powershell
cd .\HRFrontend
npm.cmd ci
```

PowerShell may block `npm.ps1`; use `npm.cmd` on Windows.

## Frontend build command

```powershell
cd .\HRFrontend
npm.cmd run build
```

## Build result

Result: Passed.

Observed output:

- TypeScript build completed.
- Vite production build completed.
- Output generated under `HRFrontend/dist`.

Known warning:

- Vite reported that one generated JavaScript chunk is larger than 500 kB after minification.
- This is a performance/code-splitting warning, not a build failure.
- No UI code was changed because the task is documentation and evidence preparation only.

## Local run command

Frontend:

```powershell
cd .\HRFrontend
npm.cmd run dev -- --host 127.0.0.1 --port 5173
```

Backend:

```powershell
dotnet run --no-build --project .\HRAPI\HRAPI.csproj --launch-profile http
```

Expected URLs:

- Frontend: `http://127.0.0.1:5173`
- Backend Swagger: `http://localhost:5065/swagger`

## Route smoke check

The backend and frontend were started locally. The Vite dev server returned HTTP 200 for each requested frontend route:

| Page | Route | Result |
|---|---|---|
| Login | `/login` | HTTP 200 |
| Home / Dashboard | `/` | HTTP 200 |
| Departments | `/departments` | HTTP 200 |
| Positions | `/positions` | HTTP 200 |
| Employees | `/employees` | HTTP 200 |
| Leave | `/leave` | HTTP 200 |
| Attendance | `/attendance` | HTTP 200 |
| Payroll | `/payroll` | HTTP 200 |
| Salary History | `/salary-history` | HTTP 200 |
| Performance Reviews | `/performance-reviews` | HTTP 200 |

## Browser testing note

The Codex in-app browser runtime crashed in this Windows sandbox before visual navigation could be completed. Because of that, visual page checks were documented as a manual screenshot checklist instead of claiming automated browser screenshots.

For the final presentation, manually open the app in a normal browser, log in as `admin@hr.com` / `Admin123!`, and capture the checklist below.

## Screenshot checklist

Collect these screenshots manually:

1. Frontend login page.
2. Successful admin login / Home page.
3. Departments page.
4. Positions page.
5. Employees page.
6. Leave page.
7. Attendance page.
8. Payroll page.
9. Salary History page.
10. Performance Reviews page.
11. Browser developer console with no critical runtime errors during demo, if possible.

## Presentation notes

In the presentation, state that:

- The frontend production build passed.
- All requested routes are implemented in `App.tsx`.
- The Vite dev server served all requested routes locally.
- A large-chunk warning remains as a known optimization item for future improvement.
- No frontend behavior was changed during final presentation preparation.
