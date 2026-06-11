using System.ComponentModel.DataAnnotations;
using HRAPI.Enums;

namespace HRAPI.DTOs.PerformanceReviews;

// DTO used to create a performance review linked to an employee and reviewer.
public class PerformanceReviewCreateDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Range(1, int.MaxValue)]
    public int ReviewerId { get; set; }

    public DateOnly ReviewDate { get; set; }

    [Range(1, 5)]
    public int? Rating { get; set; }

    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? Goals { get; set; }

    public ReviewStatus Status { get; set; }

    public DateOnly? NextReviewDate { get; set; }
}
