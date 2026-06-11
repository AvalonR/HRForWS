using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

// Department groups employees and can optionally belong to another department.
public class Department
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Self-reference supports department trees, such as HR containing Recruiting.
    public int? ParentDepartmentId { get; set; }

    [ForeignKey(nameof(ParentDepartmentId))]
    public Department? ParentDepartment { get; set; }

    public ICollection<Department> SubDepartments { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
