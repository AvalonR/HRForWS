namespace HRAPI.DTOs.LeaveTypes;

public class LeaveTypeReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DaysAllowed { get; set; }
    public bool IsPaid { get; set; }
}