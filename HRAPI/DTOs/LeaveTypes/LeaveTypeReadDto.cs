namespace HRAPI.DTOs.LeaveTypes;

// DTO returned when listing or viewing available leave types.
public class LeaveTypeReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DaysAllowed { get; set; }
    public bool IsPaid { get; set; }
}
