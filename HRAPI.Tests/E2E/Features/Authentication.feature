Feature: Authentication
  As an administrator
  I want to log in with seeded credentials
  So that I can access protected HR API endpoints

  Scenario: Admin can log in successfully
    Given the backend is running
    When the admin logs in with valid credentials
    Then the API returns a JWT token
    And protected endpoints can be accessed with the token
