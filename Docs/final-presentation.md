# HRForWS Final Presentation Script

## 1. Project idea

Speaking notes:

HRForWS is a Human Resources Management System with an ASP.NET Core Web API backend and a React/Vite frontend. The goal is to manage core HR workflows: authentication, departments, positions, employees, leave, attendance, payroll, salary history, and performance reviews.

[SCREENSHOT PLACEHOLDER: Frontend dashboard/home showing HR Management System]

## 2. Agile methodology

Speaking notes:

The project was organized around small feature areas and iterative delivery. Work was split into requirements, backend API, frontend pages, authentication, testing, and documentation. Final preparation focused on evidence, tests, and demo readiness instead of adding new business features.

[SCREENSHOT PLACEHOLDER: GitHub Project board showing Done/In Testing columns]

## 3. Team roles

Speaking notes:

Explain who worked on backend, frontend, testing, documentation, and presentation. If presenting alone, describe the roles as responsibilities: Product Owner for requirements, Developer for implementation, QA for tests, and Scrum Master for workflow tracking.

[SCREENSHOT PLACEHOLDER: Team roles table or GitHub contributors page]

## 4. GitHub workflow

Speaking notes:

Development uses `develop` as the integration branch and feature branches for focused work. Pull requests should target `develop`, not `main`. This keeps main stable and allows review before integration.

[SCREENSHOT PLACEHOLDER: GitHub branch list or pull request targeting develop]

## 5. Functional requirements

Speaking notes:

Functional requirements are documented in `Docs/requirements.md`. The implemented core includes login, role-based access, departments, positions, employees, leave types, leave requests, attendance, payroll, salary history, performance reviews, Swagger, and Postman testing support.

[SCREENSHOT PLACEHOLDER: Requirements document functional requirements section]

## 6. Non-functional requirements

Speaking notes:

The main non-functional requirements are JWT authentication, role-based authorization, SQLite persistence through Entity Framework Core, Swagger documentation, REST/HATEOAS response envelopes, service-layer architecture, automated tests, and successful frontend/backend builds.

[SCREENSHOT PLACEHOLDER: Requirements document non-functional requirements section]

## 7. User stories with acceptance criteria

Speaking notes:

User stories are written around real user behavior: admin login, department creation, position creation, employee creation, leave request validation, and attendance duplicate prevention. Acceptance criteria define expected HTTP statuses and retrieval checks.

[SCREENSHOT PLACEHOLDER: User stories/acceptance criteria summary]

## 8. BDD/Cucumber E2E test results

Speaking notes:

BDD means tests are described using Given/When/Then language. The project includes Cucumber-style `.feature` files and executable xUnit API-level E2E tests. The latest evidence shows 6 BDD E2E scenarios passed.

[SCREENSHOT PLACEHOLDER: BDD tests passed 6/6]

## 9. Unit test results

Speaking notes:

Existing backend unit/controller tests were kept and verified. The latest evidence shows 177 non-E2E tests passed, which supports confidence in service and controller behavior.

[SCREENSHOT PLACEHOLDER: Unit test result terminal showing 177 passed]

## 10. Architecture

Speaking notes:

The architecture has three main parts: React/Vite frontend, ASP.NET Core Web API backend, and SQLite database. The frontend authenticates with JWT and calls REST endpoints. Controllers delegate business rules to services, and Entity Framework Core handles persistence.

[SCREENSHOT PLACEHOLDER: Architecture diagram or architecture summary]

## 11. Database model

Speaking notes:

The main entities include Department, Position, Employee, LeaveType, LeaveRequest, Attendance, PayrollRecord, SalaryHistory, PerformanceReview, Deduction, and Identity user tables. Recruitment entities exist as planned/database models and should not be presented as completed user-facing features.

[SCREENSHOT PLACEHOLDER: Database model/entities]

## 12. Authentication and authorization

Speaking notes:

Authentication uses ASP.NET Core Identity and JWT bearer tokens. Seeded users include Admin, HRManager, TeamLead, and Employee. Authorization is role-based, so pages and endpoints are restricted according to user role.

[SCREENSHOT PLACEHOLDER: Frontend login page]

## 13. Swagger/Postman API testing

Speaking notes:

Swagger provides interactive endpoint documentation at `/swagger`. A Postman collection exists for Auth, Departments, Positions, Employees, LeaveTypes, LeaveRequests, and Attendances. Use the login request first, then apply the bearer token to protected endpoints.

[SCREENSHOT PLACEHOLDER: Swagger endpoint list]

[SCREENSHOT PLACEHOLDER: Postman collection]

## 14. Frontend demo

Speaking notes:

Demo order: log in as admin, show home, open Departments, Positions, Employees, Leave, Attendance, Payroll, Salary History, and Performance Reviews. Avoid showing planned recruitment modules as completed work.

[SCREENSHOT PLACEHOLDER: Frontend Departments page]

[SCREENSHOT PLACEHOLDER: Frontend Employees page]

[SCREENSHOT PLACEHOLDER: Frontend Leave page]

[SCREENSHOT PLACEHOLDER: Frontend Attendance page]

## 15. Challenges

Speaking notes:

Challenges included coordinating backend and frontend contracts, adding authentication without breaking existing endpoints, preserving DTO names and routes, keeping tests rerunnable with unique data, and preparing presentation evidence without adding last-minute features.

[SCREENSHOT PLACEHOLDER: Testing evidence document showing known warnings/notes]

## 16. Future improvements

Speaking notes:

Future improvements include frontend code-splitting to reduce the Vite large-chunk warning, richer dashboard analytics, improved automated UI/browser testing, CI integration, and completing planned recruitment features only after the core HR system is stable.

[SCREENSHOT PLACEHOLDER: Future improvements backlog or GitHub issues]

## 17. Conclusion

Speaking notes:

The project demonstrates a working HR system with authenticated API access, role-based frontend navigation, documented requirements, automated unit tests, BDD-style E2E tests, Swagger/Postman API testing, and a final demo path.

[SCREENSHOT PLACEHOLDER: Final application home page after login]
