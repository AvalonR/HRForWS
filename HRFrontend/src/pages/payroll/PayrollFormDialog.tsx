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
  createPayrollRecord,
  updatePayrollRecord,
} from "../../services/PayrollRecordService";
import { getEmployees } from "../../services/EmployeeService";
import type {
  PayrollRecordReadDto,
  PayrollRecordUpdateDto,
  EmployeeReadDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

const STATUS_OPTIONS = [
  { value: 0, label: "Pending" },
  { value: 1, label: "Processed" },
  { value: 2, label: "Paid" },
  { value: 3, label: "Cancelled" },
];

interface Props {
  open: boolean;
  record: PayrollRecordReadDto | null;
  onSaved: () => void;
  onClose: () => void;
}

export default function PayrollFormDialog({
  open,
  record,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!record;
  const [employeeId, setEmployeeId] = useState<number | "">("");
  const [payPeriodStart, setPayPeriodStart] = useState("");
  const [payPeriodEnd, setPayPeriodEnd] = useState("");
  const [baseSalary, setBaseSalary] = useState("");
  const [overtime, setOvertime] = useState("");
  const [bonuses, setBonuses] = useState("");
  const [deductionsTotal, setDeductionsTotal] = useState("");
  const [netPay, setNetPay] = useState("");
  const [payDate, setPayDate] = useState("");
  const [status, setStatus] = useState<number>(0);
  const [employees, setEmployees] = useState<EmployeeReadDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setEmployeeId(record?.employeeId ?? "");
      setPayPeriodStart(record?.payPeriodStart ?? "");
      setPayPeriodEnd(record?.payPeriodEnd ?? "");
      setBaseSalary(record ? String(record.baseSalary) : "");
      setOvertime(record ? String(record.overtime) : "");
      setBonuses(record ? String(record.bonuses) : "");
      setDeductionsTotal(record ? String(record.deductionsTotal) : "");
      setNetPay(record ? String(record.netPay) : "");
      setPayDate(record?.payDate ?? "");
      setStatus(record?.status ?? 0);
      setError(null);
      getEmployees()
        .then((list) => setEmployees(list))
        .catch(() => {});
    }
  }, [open, record]);

  const handleSave = async () => {
    if (saving) return;
    if (employeeId === "" || !payPeriodStart || !payPeriodEnd || !baseSalary || !netPay || !payDate) return;

    setSaving(true);
    setError(null);

    try {
      const dto = {
        employeeId: employeeId as number,
        payPeriodStart,
        payPeriodEnd,
        baseSalary: Number(baseSalary),
        overtime: overtime ? Number(overtime) : 0,
        bonuses: bonuses ? Number(bonuses) : 0,
        deductionsTotal: deductionsTotal ? Number(deductionsTotal) : 0,
        netPay: Number(netPay),
        payDate,
        status,
      };

      if (isEdit) {
        await updatePayrollRecord(record!.id, dto as PayrollRecordUpdateDto);
      } else {
        await createPayrollRecord(dto);
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
      <DialogTitle>{isEdit ? "Edit Payroll Record" : "Add Payroll Record"}</DialogTitle>
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
          label="Pay Period Start"
          type="date"
          value={payPeriodStart}
          onChange={(e) => setPayPeriodStart(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Pay Period End"
          type="date"
          value={payPeriodEnd}
          onChange={(e) => setPayPeriodEnd(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Base Salary"
          value={baseSalary}
          onChange={(e) => setBaseSalary(e.target.value)}
          type="number"
          fullWidth
          required
          margin="normal"
        />
        <TextField
          label="Overtime"
          value={overtime}
          onChange={(e) => setOvertime(e.target.value)}
          type="number"
          fullWidth
          margin="normal"
        />
        <TextField
          label="Bonuses"
          value={bonuses}
          onChange={(e) => setBonuses(e.target.value)}
          type="number"
          fullWidth
          margin="normal"
        />
        <TextField
          label="Deductions Total"
          value={deductionsTotal}
          onChange={(e) => setDeductionsTotal(e.target.value)}
          type="number"
          fullWidth
          margin="normal"
        />
        <TextField
          label="Net Pay"
          value={netPay}
          onChange={(e) => setNetPay(e.target.value)}
          type="number"
          fullWidth
          required
          margin="normal"
        />
        <TextField
          label="Pay Date"
          type="date"
          value={payDate}
          onChange={(e) => setPayDate(e.target.value)}
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
        {error && (
          <Typography color="error" sx={{ mt: 1 }}>{error}</Typography>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={saving}>Cancel</Button>
        <Button
          variant="contained"
          onClick={handleSave}
          disabled={employeeId === "" || !payPeriodStart || !payPeriodEnd || !baseSalary || !netPay || !payDate || saving}
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
