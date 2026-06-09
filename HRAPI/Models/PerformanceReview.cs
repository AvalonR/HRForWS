using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

public class PerformanceReview
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public int ReviewerId { get; set; }

    [ForeignKey(nameof(ReviewerId))]
    public Employee Reviewer { get; set; } = null!;

    public DateOnly ReviewDate { get; set; }

    public int? Rating { get; set; }

    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? Goals { get; set; }

    public ReviewStatus Status { get; set; }

    public DateOnly? NextReviewDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
