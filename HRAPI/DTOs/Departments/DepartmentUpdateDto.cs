using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Departments;

// DTO used when updating a department; includes editable fields and active status.
public class DepartmentUpdateDto
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

    public bool IsActive { get; set; } = true;
}
