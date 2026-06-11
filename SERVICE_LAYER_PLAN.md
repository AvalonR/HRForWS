# Service Layer Refactor Plan

## Goal
Extract business logic from controllers into dedicated services to follow Single Responsibility Principle.

## Problem
Controllers currently handle: HTTP, validation, data access, entity mapping, and business rules. This violates SRP and makes unit testing difficult.

## Solution
One service class per entity with a shared interface pattern. Controllers become thin — HTTP only.

## Layer structure

```
Controllers/          → thin, ~30 lines each, only HTTP concerns
Services/
  Interfaces/
    IDepartmentService.cs
    IEmployeeService.cs
    IPositionService.cs
    ILeaveTypeService.cs
    IAuthService.cs
  DepartmentService.cs
  EmployeeService.cs
  PositionService.cs
  LeaveTypeService.cs
  AuthService.cs
```

## Service contract pattern

```csharp
Task<List<TReadDto>> GetAllAsync();
Task<TReadDto?> GetByIdAsync(int id);
Task<TReadDto> CreateAsync(TCreateDto dto);
Task UpdateAsync(int id, TUpdateDto dto);
Task DeleteAsync(int id);
```

## Steps

1. Create `Services/Interfaces/` folder
2. Define interface per entity with CRUD contracts
3. Implement each service class — move all `_context` queries, validation rules, and entity→DTO mapping out of controllers
4. Inject services into controllers (replace direct `AppDbContext` injection)
5. AuthService: wraps `UserManager<AppUser>` + `SignInManager<AppUser>` + JWT generation
6. Register all services in `Program.cs` via `AddScoped`
7. Remove `[Authorize]` from controllers once services handle auth logic

## Per-entity notes

| Entity | Validation highlights |
|--------|----------------------|
| Department | Unique code, no self-parent, cascade check on delete |
| Employee | Unique employee number + email, department/position/must exist, no self-manager |
| Position | Min ≤ Max salary, unique (title + department), department must exist |
| LeaveType | Unique name, dependency check on delete |

## PR
This refactor will be its own PR after auth is complete and merged.
