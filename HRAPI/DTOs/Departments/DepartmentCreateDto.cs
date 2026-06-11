using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Departments;

// DTO used when creating a department; clients cannot set server-controlled fields like Id or CreatedAt.
public class DepartmentCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? ParentDepartmentId { get; set; }
}
