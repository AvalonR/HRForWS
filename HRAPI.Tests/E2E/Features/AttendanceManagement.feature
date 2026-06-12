Feature: Attendance management
  As an administrator
  I want duplicate attendance records to be rejected
  So that each employee has only one attendance record per date

  Scenario: Attendance cannot be duplicated for the same employee and date
    Given an authenticated admin user
    And an existing employee
    When an attendance record is created for a date
    And another attendance record is created for the same employee and date
    Then the second response status is Bad Request
