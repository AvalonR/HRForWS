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
import {
  createAttendance,
  updateAttendance,
} from "../../services/AttendanceService";
import { getEmployees } from "../../services/EmployeeService";
import type {
  AttendanceReadDto,
  AttendanceUpdateDto,
  EmployeeReadDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

const STATUS_OPTIONS = [
  { value: 0, label: "Present" },
  { value: 1, label: "Absent" },
  { value: 2, label: "Late" },
  { value: 3, label: "HalfDay" },
  { value: 4, label: "Remote" },
  { value: 5, label: "OnLeave" },
];

interface Props {
  open: boolean;
  attendance: AttendanceReadDto | null;
  onSaved: () => void;
  onClose: () => void;
}

export default function AttendanceFormDialog({
  open,
  attendance,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!attendance;
  const [employeeId, setEmployeeId] = useState<number | "">("");
  const [date, setDate] = useState("");
  const [status, setStatus] = useState<number>(0);
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");
  const [notes, setNotes] = useState("");
  const [employees, setEmployees] = useState<EmployeeReadDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setEmployeeId(attendance?.employeeId ?? "");
      setDate(attendance?.date ?? "");
      setStatus(attendance?.status ?? 0);
      setCheckIn(attendance?.checkIn ?? "");
      setCheckOut(attendance?.checkOut ?? "");
      setNotes(attendance?.notes ?? "");
      setError(null);
      getEmployees()
        .then((list) => setEmployees(list))
        .catch(() => {});
    }
  }, [open, attendance]);

  const handleSave = async () => {
    if (saving) return;
    if (employeeId === "" || !date) return;

    setSaving(true);
    setError(null);

    try {
      const dto = {
        employeeId: employeeId as number,
        date,
        status,
        checkIn: checkIn || undefined,
        checkOut: checkOut || undefined,
        notes: notes.trim() || undefined,
      };

      if (isEdit) {
        await updateAttendance(attendance!.id, dto as AttendanceUpdateDto);
      } else {
        await createAttendance(dto);
      }
      onSaved();
    } catch (err: unknown) {
      const message = getErrorMessage(err, "An error occurred.");
      setError(message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? "Edit Attendance" : "Add Attendance"}</DialogTitle>
      <DialogContent>
        <TextField
          select
          label="Employee"
          value={employeeId}
          onChange={(e) =>
            setEmployeeId(e.target.value === "" ? "" : Number(e.target.value))
          }
          fullWidth
          required
          margin="normal"
          autoFocus
        >
          <MenuItem value="" disabled>Select employee</MenuItem>
          {employees.map((e) => (
            <MenuItem key={e.id} value={e.id}>{e.fullName}</MenuItem>
          ))}
        </TextField>
        <TextField
          label="Date"
          type="date"
          value={date}
          onChange={(e) => setDate(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          select
          label="Status"
          value={status}
          onChange={(e) => setStatus(Number(e.target.value))}
          fullWidth
          required
          margin="normal"
        >
          {STATUS_OPTIONS.map((o) => (
            <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>
          ))}
        </TextField>
        <TextField
          label="Check In"
          type="time"
          value={checkIn}
          onChange={(e) => setCheckIn(e.target.value)}
          fullWidth
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Check Out"
          type="time"
          value={checkOut}
          onChange={(e) => setCheckOut(e.target.value)}
          fullWidth
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Notes"
          value={notes}
          onChange={(e) => setNotes(e.target.value)}
          fullWidth
          multiline
          rows={2}
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
          disabled={employeeId === "" || !date || saving}
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
