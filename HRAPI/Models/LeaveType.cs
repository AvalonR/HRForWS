using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRAPI.Models;

// Defines a category of leave and how many days are allowed.
public class LeaveType
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int DaysAllowed { get; set; }

    public bool IsPaid { get; set; }
}
