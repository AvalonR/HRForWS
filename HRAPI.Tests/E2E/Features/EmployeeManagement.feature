Feature: Employee management
  As an administrator
  I want to create employees
  So that HR records can be managed in the system

  Scenario: Admin can create an employee
    Given an authenticated admin user
    And an existing department
    And an existing position
    When a new employee is created
    Then the response status is Created
    And the employee can be retrieved from the API
