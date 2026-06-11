namespace HRAPI.Enums;

// Standard attendance values avoid free-text differences like "late" vs "Late".
public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    HalfDay,
    Remote,
    OnLeave
}
