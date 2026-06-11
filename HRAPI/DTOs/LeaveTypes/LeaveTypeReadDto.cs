namespace HRAPI.DTOs.LeaveTypes;

// DTO returned when listing or viewing leave type definitions.
public class LeaveTypeReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DaysAllowed { get; set; }
    public bool IsPaid { get; set; }
}