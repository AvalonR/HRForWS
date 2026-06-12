Feature: Position management
  As an administrator
  I want to create positions for departments
  So that employees can be assigned to valid roles

  Scenario: Admin can create a position for an existing department
    Given an authenticated admin user
    And an existing department
    When a new position is created for that department
    Then the response status is Created
    And the position can be retrieved from the API
