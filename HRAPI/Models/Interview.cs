using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

// Existing recruiting model for interview scheduling and feedback.
public class Interview
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    [ForeignKey(nameof(ApplicationId))]
    public Application Application { get; set; } = null!;

    public int? InterviewerId { get; set; }

    [ForeignKey(nameof(InterviewerId))]
    public Employee? Interviewer { get; set; }

    public DateTime InterviewDate { get; set; }

    public InterviewType Type { get; set; }

    public int? Rating { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
