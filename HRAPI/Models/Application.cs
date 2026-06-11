using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRAPI.Enums;

namespace HRAPI.Models;

// Recruiting model connecting an applicant to a job posting.
public class Application
{
    public int Id { get; set; }

    [Required]
    public int JobPostingId { get; set; }

    [ForeignKey(nameof(JobPostingId))]
    public JobPosting JobPosting { get; set; } = null!;

    [Required]
    public int ApplicantId { get; set; }

    [ForeignKey(nameof(ApplicantId))]
    public Applicant Applicant { get; set; } = null!;

    public DateTime ApplicationDate { get; set; }

    public ApplicationStatus Status { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
