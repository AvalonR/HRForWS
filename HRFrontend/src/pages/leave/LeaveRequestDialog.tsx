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
import { createLeaveRequest } from "../../services/LeaveRequestService";
import { getEmployees } from "../../services/EmployeeService";
import type {
  LeaveTypeReadDto,
  EmployeeReadDto,
} from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";

interface Props {
  open: boolean;
  leaveTypes: LeaveTypeReadDto[];
  onCreated: () => void;
  onClose: () => void;
}

export default function LeaveRequestDialog({
  open,
  leaveTypes,
  onCreated,
  onClose,
}: Props) {
  const { user } = useAuth();
  const isAdminOrHr =
    user?.roles.includes("Admin") || user?.roles.includes("HRManager");
  const [employeeId, setEmployeeId] = useState<number | "">("");
  const [leaveTypeId, setLeaveTypeId] = useState<number | "">("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [reason, setReason] = useState("");
  const [employees, setEmployees] = useState<EmployeeReadDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setEmployeeId("");
      setLeaveTypeId("");
      setStartDate("");
      setEndDate("");
      setReason("");
      setError(null);
      if (isAdminOrHr) {
        getEmployees()
          .then((list) => setEmployees(list))
          .catch(() => {});
      }
    }
  }, [open, isAdminOrHr]);

  const handleSave = async () => {
    if (saving) return;
    if (leaveTypeId === "" || !startDate || !endDate) return;
    if (isAdminOrHr && employeeId === "") return;
    if (!isAdminOrHr && user?.employeeId == null) {
      setError("No employee profile linked to your account.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const dto = {
        employeeId: isAdminOrHr ? (employeeId as number) : user!.employeeId!,
        leaveTypeId: leaveTypeId as number,
        startDate,
        endDate,
        reason: reason.trim() || undefined,
      };
      await createLeaveRequest(dto);
      onCreated();
    } catch (err: unknown) {
      const message = getErrorMessage(err, "An error occurred.");
      setError(message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>New Leave Request</DialogTitle>
      <DialogContent>
        {isAdminOrHr && (
          <TextField
            select
            label="Employee"
            value={employeeId}
            onChange={(e) =>
              setEmployeeId(
                e.target.value === "" ? "" : Number(e.target.value),
              )
            }
            fullWidth
            required
            margin="normal"
          >
            <MenuItem value="" disabled>
              Select employee
            </MenuItem>
            {employees.map((e) => (
              <MenuItem key={e.id} value={e.id}>
                {e.fullName}
              </MenuItem>
            ))}
          </TextField>
        )}
        <TextField
          select
          label="Leave Type"
          value={leaveTypeId}
          onChange={(e) =>
            setLeaveTypeId(
              e.target.value === "" ? "" : Number(e.target.value),
            )
          }
          fullWidth
          required
          margin="normal"
        >
          <MenuItem value="" disabled>
            Select leave type
          </MenuItem>
          {leaveTypes.map((lt) => (
            <MenuItem key={lt.id} value={lt.id}>
              {lt.name} ({lt.daysAllowed} days)
            </MenuItem>
          ))}
        </TextField>
        <TextField
          label="Start Date"
          type="date"
          value={startDate}
          onChange={(e) => setStartDate(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{
            inputLabel: { shrink: true },
            htmlInput: { min: new Date().toLocaleDateString("en-CA") },
          }}
        />
        <TextField
          label="End Date"
          type="date"
          value={endDate}
          onChange={(e) => setEndDate(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{
            inputLabel: { shrink: true },
            htmlInput: { min: startDate || new Date().toLocaleDateString("en-CA") },
          }}
        />
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
          <Typography color="error" sx={{ mt: 1 }}>
            {error}
          </Typography>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={saving}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={handleSave}
          disabled={
            leaveTypeId === "" ||
            !startDate ||
            !endDate ||
            (isAdminOrHr && employeeId === "") ||
            saving
          }
        >
          {saving ? <CircularProgress size={20} /> : "Submit"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
