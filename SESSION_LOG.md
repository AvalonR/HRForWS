# HRForWS — Session Log

## Session 1 — 2026-06-09

### Summary
Designed and implemented the full database schema for a RESTful HR Web Service (.NET 10 / EF Core / SQLite).

### Done
- Added NuGet packages: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`
- Created 14 entity model classes in `Models/`
  - Department, Position, Employee, LeaveType, LeaveRequest, Attendance
  - SalaryHistory, PayrollRecord, Deduction, PerformanceReview
  - JobPosting, Applicant, Application, Interview
- All models follow consistent FK + navigation property pattern
- Review fixes applied: typo `AppllicationId` -> `ApplicationId`, missing `Status` on JobPosting

### Pending (next session)
1. Create `Data/AppDbContext.cs` with:
   - 14 `DbSet<>` properties
   - Fluent API: unique indexes, decimal precision, cascade behavior (Restrict for self-refs)
   - Seed data (LeaveTypes)
2. Add `ConnectionStrings:DefaultConnection` to `appsettings.json`
3. Register `DbContext` in `Program.cs`
4. Run `dotnet ef migrations add InitialCreate`
5. Run `dotnet ef database update`
6. Add `*.db` to `.gitignore`

### Team Notes
- Assignment confirms .NET is acceptable
- Richardson Level 4 is actually Level 3 (HATEOAS) — verify with instructor
- Each team member builds out their domain (controllers/services/tests) after the schema is merged to master

---

## Session 2 — 2026-06-11

### Summary
Started frontend development with React + Vite + TypeScript + MUI. Built the Department management CRUD UI and sidebar navigation.

### Done
- Created `HRFrontend/` with Vite + React + TypeScript scaffold
- Installed dependencies: `@mui/material`, `@mui/icons-material`, `@mui/x-data-grid`, `@emotion/react`, `@emotion/styled`, `axios`, `react-router-dom`
- Set up folder structure: `types/`, `services/`, `pages/{entity}/`, `components/`, `layouts/`
- Configured MUI theme (`#1565c0` primary) and CssBaseline
- Created Axios API client pointing to `http://localhost:5065/api`
- Created TypeScript DTOs matching backend: `DepartmentReadDto`, `DepartmentCreateDto`, `DepartmentUpdateDto`
- Built `DepartmentService.ts` with all 5 CRUD functions
- Fixed CORS in `HRAPI/Program.cs` — added `AllowFrontend` policy for `localhost:5173`
- Built `DepartmentsList.tsx` — MUI DataGrid with columns (Code, Name, Description, Parent Dept, Status, Actions), loading/error/empty states, pagination
- Built `DepartmentFormDialog.tsx` — create/edit dialog with validation
- Built `DeleteConfirmDialog.tsx` — reusable confirmation dialog
- Built `MainLayout.tsx` — persistent sidebar navigation with icons for all 6 V1 entities
- Updated `App.tsx` to use nested routes with layout
- Added `*.db-shm` and `*.db-wal` to root `.gitignore`
- Removed empty `DepartmentForm.tsx`

### Branch
- `feature/frontend-department-management` — created, committed, and pushed
- PR opened against `develop` (not yet merged)

### Pending (next session)
1. Create GitHub Issues for frontend tickets (FT-02 through FT-17)
2. Build Position management UI (next entity)
3. Build Employee management UI
4. Build LeaveType, LeaveRequest, Attendance UIs
