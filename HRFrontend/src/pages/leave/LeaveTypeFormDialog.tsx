import { useState, useEffect } from "react";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";
import TextField from "@mui/material/TextField";
import FormControlLabel from "@mui/material/FormControlLabel";
import Switch from "@mui/material/Switch";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import CircularProgress from "@mui/material/CircularProgress";
import {
  createLeaveType,
  updateLeaveType,
} from "../../services/LeaveTypeService";
import type {
  LeaveTypeReadDto,
  LeaveTypeCreateDto,
  LeaveTypeUpdateDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

interface Props {
  open: boolean;
  leaveType: LeaveTypeReadDto | null;
  onSaved: () => void;
  onClose: () => void;
}

export default function LeaveTypeFormDialog({
  open,
  leaveType,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!leaveType;
  const [name, setName] = useState("");
  const [daysAllowed, setDaysAllowed] = useState<string>("");
  const [isPaid, setIsPaid] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setName(leaveType?.name ?? "");
      setDaysAllowed(leaveType ? String(leaveType.daysAllowed) : "");
      setIsPaid(leaveType?.isPaid ?? true);
      setError(null);
    }
  }, [open, leaveType]);

  const handleSave = async () => {
    if (saving) return;
    if (!name.trim() || !daysAllowed) return;

    setSaving(true);
    setError(null);

    try {
      if (isEdit) {
        const dto: LeaveTypeUpdateDto = {
          name: name.trim(),
          daysAllowed: Number(daysAllowed),
          isPaid,
        };
        await updateLeaveType(leaveType!.id, dto);
      } else {
        const dto: LeaveTypeCreateDto = {
          name: name.trim(),
          daysAllowed: Number(daysAllowed),
          isPaid,
        };
        await createLeaveType(dto);
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
      <DialogTitle>{isEdit ? "Edit Leave Type" : "Add Leave Type"}</DialogTitle>
      <DialogContent>
        <TextField
          label="Name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          fullWidth
          required
          margin="normal"
          autoFocus
        />
        <TextField
          label="Days Allowed"
          value={daysAllowed}
          onChange={(e) => setDaysAllowed(e.target.value)}
          type="number"
          fullWidth
          required
          margin="normal"
          slotProps={{ htmlInput: { min: 1, max: 365 } }}
        />
        <FormControlLabel
          control={<Switch checked={isPaid} onChange={(e) => setIsPaid(e.target.checked)} />}
          label="Paid Leave"
          sx={{ mt: 1 }}
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
          disabled={!name.trim() || !daysAllowed || saving}
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
