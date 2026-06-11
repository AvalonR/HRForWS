using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

// Existing recruiting model for job advertisements; not part of the current core HR focus.
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

    public EmploymentType EmploymentType { get; set; }

    public JobPostingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
