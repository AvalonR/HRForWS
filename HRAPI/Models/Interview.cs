using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

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

    public string Type { get; set; } = string.Empty;

    public int? Rating { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
