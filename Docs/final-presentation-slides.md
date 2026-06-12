# Final Presentation Slides

## Slide 1: HRForWS project idea

Bullet points:

- Human Resources Management System
- ASP.NET Core Web API backend
- React/Vite frontend
- Core HR workflows and role-based access

Speaker notes:

Introduce the system and explain that the goal is to manage everyday HR data through a web API and frontend.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Frontend dashboard/home]

Manual image to add:

Frontend home page after admin login.

## Slide 2: Agile methodology

Bullet points:

- Iterative feature delivery
- Requirements first, then implementation
- Testing and documentation in final phase
- Focused final scope

Speaker notes:

Explain how work was split into manageable increments and finalized with testing evidence.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: GitHub Project board showing Done/In Testing columns]

Manual image to add:

GitHub Project board.

## Slide 3: Team roles

Bullet points:

- Product Owner: requirements and priorities
- Developer: backend/frontend implementation
- QA: unit and E2E testing
- Scrum Master: workflow and presentation readiness

Speaker notes:

Describe actual team members or responsibilities depending on presentation format.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Team roles or contributors]

Manual image to add:

Contributor list, roles table, or team slide.

## Slide 4: GitHub workflow

Bullet points:

- `main` remains stable
- `develop` is the integration branch
- Feature branches for focused work
- Pull requests target `develop`

Speaker notes:

Show the branch strategy and explain why PRs should not go directly into main.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: GitHub branch/PR workflow]

Manual image to add:

Branch list or PR screen.

## Slide 5: Requirements

Bullet points:

- Functional requirements documented
- Non-functional requirements documented
- Scope separates completed and planned features
- Demo focuses on completed core HR workflows

Speaker notes:

Point to `Docs/requirements.md` as the source of truth.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Requirements document]

Manual image to add:

Requirements document in editor or GitHub.

## Slide 6: Functional requirements

Bullet points:

- Login and JWT authentication
- Department, position, employee management
- Leave and attendance management
- Payroll, salary history, performance reviews

Speaker notes:

Summarize what users can do in the system today.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Functional requirements table]

Manual image to add:

Functional requirements section.

## Slide 7: Non-functional requirements

Bullet points:

- JWT and role-based authorization
- SQLite and EF Core
- Swagger API documentation
- Automated tests and build verification

Speaker notes:

Explain the quality attributes that make the system secure, testable, and demonstrable.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Non-functional requirements table]

Manual image to add:

Non-functional requirements section.

## Slide 8: User stories and acceptance criteria

Bullet points:

- Admin can log in
- Admin can create department/position/employee
- Invalid leave dates are rejected
- Duplicate attendance is rejected

Speaker notes:

Connect the requirements to testable user behavior.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: User stories/acceptance criteria]

Manual image to add:

Presentation prep or E2E testing doc section.

## Slide 9: BDD/Cucumber E2E tests

Bullet points:

- Given/When/Then feature files
- API-level E2E execution
- Real seeded authentication
- Result: 6 passed, 0 failed

Speaker notes:

Explain that the scenarios are written in Cucumber-style Gherkin and executed through xUnit HTTP tests.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: BDD E2E test result terminal showing 6 passed]

Manual image to add:

Terminal output from E2E test run.

## Slide 10: Unit tests

Bullet points:

- Existing backend test suite verified
- Service/controller behavior covered
- Result: 177 passed, 0 failed

Speaker notes:

Show that the new documentation and E2E work did not break existing tests.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Unit test result terminal showing 177 passed]

Manual image to add:

Terminal output from non-E2E test run.

## Slide 11: Architecture

Bullet points:

- React/Vite frontend
- ASP.NET Core Web API
- Service layer
- EF Core with SQLite

Speaker notes:

Walk through how requests move from frontend to API controllers, services, and database.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Architecture diagram or architecture summary]

Manual image to add:

Architecture diagram or simple system flow.

## Slide 12: Database model

Bullet points:

- Department, Position, Employee
- LeaveType, LeaveRequest, Attendance
- Payroll, SalaryHistory, PerformanceReview
- Identity users and roles

Speaker notes:

Explain the core relationships and note that recruitment models are future/planned.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Database model/entities]

Manual image to add:

Entity diagram or database model document.

## Slide 13: Authentication and authorization

Bullet points:

- Seeded demo accounts
- JWT bearer tokens
- Role-based backend endpoints
- Role-based frontend navigation

Speaker notes:

Log in as admin and show how role access controls the system.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Frontend login page]

Manual image to add:

Login page and/or authenticated user menu.

## Slide 14: Swagger and Postman

Bullet points:

- Swagger available at `/swagger`
- Postman collection included
- Login request stores bearer token
- Main API endpoint groups covered

Speaker notes:

Show Swagger first, then Postman collection structure.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Swagger endpoint list]

[SCREENSHOT PLACEHOLDER: Postman collection]

Manual image to add:

Swagger page and Postman collection sidebar.

## Slide 15: Frontend demo

Bullet points:

- Login
- Home
- Departments, Positions, Employees
- Leave, Attendance, Payroll, Salary History, Reviews

Speaker notes:

Use this as the live demo checklist.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Frontend Departments page]

[SCREENSHOT PLACEHOLDER: Frontend Employees page]

[SCREENSHOT PLACEHOLDER: Frontend Leave page]

[SCREENSHOT PLACEHOLDER: Frontend Attendance page]

Manual image to add:

Screenshots of key frontend pages.

## Slide 16: Challenges and future improvements

Bullet points:

- Contract alignment between API and frontend
- Authentication integration
- Stable rerunnable test data
- Future: code splitting, CI, UI browser tests, recruitment features

Speaker notes:

Close by explaining what was learned and what would be improved next.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Testing evidence and future backlog]

Manual image to add:

Testing evidence document or GitHub backlog.

## Slide 17: Conclusion

Bullet points:

- Requirements documented
- Core HR app implemented
- Swagger/Postman testing ready
- 6 BDD E2E tests passed
- 177 unit tests passed

Speaker notes:

End with the evidence that the project is ready to demonstrate.

Screenshot placeholder:

[SCREENSHOT PLACEHOLDER: Final application home page]

Manual image to add:

Final home page or summary screenshot.
