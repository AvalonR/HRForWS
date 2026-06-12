import { useState, useEffect } from "react";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";
import TextField from "@mui/material/TextField";
import MenuItem from "@mui/material/MenuItem";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import CircularProgress from "@mui/material/CircularProgress";
import { updateLeaveRequest } from "../../services/LeaveRequestService";
import { useAuth } from "../../contexts/AuthContext";
import type {
  LeaveRequestReadDto,
  LeaveRequestUpdateDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

interface Props {
  open: boolean;
  request: LeaveRequestReadDto;
  onSaved: () => void;
  onClose: () => void;
}

export default function LeaveRequestUpdateDialog({
  open,
  request,
  onSaved,
  onClose,
}: Props) {
  const { user } = useAuth();
  const [status, setStatus] = useState<number>(request.status);
  const [reason, setReason] = useState(request.reason ?? "");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setStatus(request.status);
      setReason(request.reason ?? "");
      setError(null);
    }
  }, [open, request]);

  const handleSave = async () => {
    if (saving || !request) return;
    setSaving(true);
    setError(null);

    try {
      const dto: LeaveRequestUpdateDto = {
        employeeId: request.employeeId,
        leaveTypeId: request.leaveTypeId,
        startDate: request.startDate,
        endDate: request.endDate,
        status,
        reason: reason.trim() || undefined,
        reviewedByEmployeeId: request.reviewedByEmployeeId ?? undefined,
      };
      await updateLeaveRequest(request.id, dto);
      onSaved();
    } catch (err: unknown) {
      const message = getErrorMessage(err, "An error occurred.");
      setError(message);
    } finally {
      setSaving(false);
    }
  };

  const canApprove =
    user?.roles.includes("Admin") || user?.roles.includes("HRManager");

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        Leave Request — {request.employeeName}
      </DialogTitle>
      <DialogContent>
        <TextField
          label="Leave Type"
          value={request.leaveTypeName}
          fullWidth
          margin="normal"
          slotProps={{ input: { readOnly: true } }}
        />
        <TextField
          label="Start Date"
          value={request.startDate}
          fullWidth
          margin="normal"
          slotProps={{ input: { readOnly: true } }}
        />
        <TextField
          label="End Date"
          value={request.endDate}
          fullWidth
          margin="normal"
          slotProps={{ input: { readOnly: true } }}
        />
        <TextField
          select
          label="Status"
          value={status}
          onChange={(e) => setStatus(Number(e.target.value))}
          fullWidth
          required
          margin="normal"
          disabled={!canApprove}
        >
          <MenuItem key={0} value={0}>Pending</MenuItem>
          <MenuItem key={1} value={1}>Approved</MenuItem>
          <MenuItem key={2} value={2}>Rejected</MenuItem>
          <MenuItem key={3} value={3}>Cancelled</MenuItem>
        </TextField>
        <TextField
          label="Reason"
          value={reason}
          onChange={(e) => setReason(e.target.value)}
          fullWidth
          multiline
          rows={3}
          margin="normal"
        />
        {error && (
          <Typography color="error" sx={{ mt: 1 }}>{error}</Typography>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={saving}>Cancel</Button>
        <Button
          variant="contained"
          onClick={handleSave}
          disabled={saving}
        >
          {saving ? <CircularProgress size={20} /> : "Update"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
