Feature: Department management
  As an administrator
  I want to create and retrieve departments
  So that the organization structure can be maintained

  Scenario: Admin can create and retrieve a department
    Given an authenticated admin user
    When a new department is created
    Then the response status is Created
    And the department can be retrieved from the API
