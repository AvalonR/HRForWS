using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

public class Employee
{
    public int Id { get; set; }

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

    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;

    public int PositionId { get; set; }

    [ForeignKey(nameof(PositionId))]
    public Position Position { get; set; } = null!;

    public int? ManagerId { get; set; }
    [ForeignKey(nameof(ManagerId))]
    public Employee? Manager { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
