using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Departments;

// DTO used to create a department without allowing clients to set Id or CreatedAt.
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