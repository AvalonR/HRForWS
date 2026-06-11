namespace HRAPI.Enums;

// Leave request statuses describe the approval workflow from pending to final decision.
public enum LeaveRequestStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}
