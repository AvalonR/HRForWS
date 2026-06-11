using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Employees;

// DTO used to update employee profile, department, position, manager, and active status.
public class EmployeeUpdateDto
{
    [Required]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Required]
    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public int PositionId { get; set; }

    public int? ManagerId { get; set; }

    public bool IsActive { get; set; } = true;
}
