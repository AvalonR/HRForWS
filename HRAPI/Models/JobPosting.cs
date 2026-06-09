using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

public class JobPosting
{
    public int Id { get; set; }

    public int PositionId { get; set; }

    [ForeignKey(nameof(PositionId))]
    public Position Position { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateOnly PostingDate { get; set; }

    public DateOnly? ClosingDate { get; set; }

    public string EmploymentType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
