using HRAPI.Enums;

namespace HRAPI.DTOs.PerformanceReviews;

public class PerformanceReviewReadDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
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
