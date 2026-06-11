namespace HRAPI.Enums;

// Attendance statuses make records consistent instead of storing free-text values.
public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    HalfDay,
    Remote,
    OnLeave
}
