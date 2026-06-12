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
  createSalaryHistory,
  updateSalaryHistory,
} from "../../services/SalaryHistoryService";
import { getEmployees } from "../../services/EmployeeService";
import type {
  SalaryHistoryReadDto,
  SalaryHistoryUpdateDto,
  EmployeeReadDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

interface Props {
  open: boolean;
  record: SalaryHistoryReadDto | null;
  onSaved: () => void;
  onClose: () => void;
}

export default function SalaryHistoryFormDialog({
  open,
  record,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!record;
  const [employeeId, setEmployeeId] = useState<number | "">("");
  const [amount, setAmount] = useState("");
  const [effectiveFrom, setEffectiveFrom] = useState("");
  const [effectiveTo, setEffectiveTo] = useState("");
  const [changeReason, setChangeReason] = useState("");
  const [employees, setEmployees] = useState<EmployeeReadDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setEmployeeId(record?.employeeId ?? "");
      setAmount(record ? String(record.amount) : "");
      setEffectiveFrom(record?.effectiveFrom ?? "");
      setEffectiveTo(record?.effectiveTo ?? "");
      setChangeReason(record?.changeReason ?? "");
      setError(null);
      getEmployees()
        .then((list) => setEmployees(list))
        .catch(() => {});
    }
  }, [open, record]);

  const handleSave = async () => {
    if (saving) return;
    if (employeeId === "" || !amount || !effectiveFrom) return;

    setSaving(true);
    setError(null);

    try {
      const dto = {
        employeeId: employeeId as number,
        amount: Number(amount),
        effectiveFrom,
        effectiveTo: effectiveTo || undefined,
        changeReason: changeReason.trim() || undefined,
      };

      if (isEdit) {
        await updateSalaryHistory(record!.id, dto as SalaryHistoryUpdateDto);
      } else {
        await createSalaryHistory(dto);
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
      <DialogTitle>{isEdit ? "Edit Salary History" : "Add Salary History"}</DialogTitle>
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
          label="Amount"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
          type="number"
          fullWidth
          required
          margin="normal"
        />
        <TextField
          label="Effective From"
          type="date"
          value={effectiveFrom}
          onChange={(e) => setEffectiveFrom(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Effective To"
          type="date"
          value={effectiveTo}
          onChange={(e) => setEffectiveTo(e.target.value)}
          fullWidth
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Change Reason"
          value={changeReason}
          onChange={(e) => setChangeReason(e.target.value)}
          fullWidth
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
          disabled={employeeId === "" || !amount || !effectiveFrom || saving}
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
