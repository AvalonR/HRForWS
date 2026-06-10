# Database Models Review

## Purpose

This document reviews the initial database models created for the HR System Web Services project.

The purpose of this review is to confirm the database structure before creating REST API controllers and endpoints.

## Existing Model Classes

The project currently contains the following model classes:

- Applicant
- Application
- Attendance
- Deduction
- Department
- Employee
- Interview
- JobPosting
- LeaveRequest
- LeaveType
- PayrollRecord
- PerformanceReview
- Position
- SalaryHistory

## Existing Enum Classes

The project currently contains the following enum classes:

- ApplicationStatus
- AttendanceStatus
- DeductionType
- EmploymentType
- InterviewType
- JobPostingStatus
- LeaveRequestStatus
- PayrollStatus
- ReviewStatus

## Version 1 Scope

For the first working version of the API, the team will focus on the core HR management entities:

- Department
- Position
- Employee
- LeaveType
- LeaveRequest
- Attendance

These entities are selected because they are enough to demonstrate a functional HR management system with database relationships, CRUD operations, Swagger testing, Postman testing, and future automated tests.

## Optional / Future Scope

The following entities are considered optional or future features:

- Applicant
- Application
- Interview
- JobPosting
- PayrollRecord
- Deduction
- PerformanceReview
- SalaryHistory

These entities may be added later if time allows, but they are not required for the first working version of the API.

## Main Relationships

The expected main relationships for Version 1 are:

- One Department can have many Employees.
- One Position can have many Employees.
- One Employee can have many LeaveRequests.
- One LeaveType can have many LeaveRequests.
- One Employee can have many Attendance records.

## Decision

The team will first create API controllers for the Version 1 entities.

The first controllers should be created in this order:

1. DepartmentsController
2. PositionsController
3. EmployeesController
4. LeaveTypesController
5. LeaveRequestsController
6. AttendanceController

## Review Result

The initial database structure is accepted for the first API version, but the API development will focus only on the Version 1 core entities first.