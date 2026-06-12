Feature: Leave request management
  As an authenticated user
  I want leave request dates to be validated
  So that invalid leave requests are rejected

  Scenario: Leave request cannot have an end date before the start date
    Given an authenticated user
    And an existing employee
    And an existing leave type
    When a leave request is submitted with an end date before the start date
    Then the response status is Bad Request
