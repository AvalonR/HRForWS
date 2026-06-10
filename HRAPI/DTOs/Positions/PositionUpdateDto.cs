using System.ComponentModel.DataAnnotations;

namespace HRAPI.DTOs.Positions;

public class PositionUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal? MinSalary { get; set; }

    public decimal? MaxSalary { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;
}