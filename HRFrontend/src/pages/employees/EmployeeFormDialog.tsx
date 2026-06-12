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
  createEmployee,
  updateEmployee,
} from "../../services/EmployeeService";
import { getDepartments } from "../../services/DepartmentService";
import { getPositions } from "../../services/PositionService";
import { getEmployees } from "../../services/EmployeeService";
import type {
  EmployeeReadDto,
  DepartmentReadDto,
  PositionReadDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

interface Props {
  open: boolean;
  employee: EmployeeReadDto | null;
  onSaved: () => void;
  onClose: () => void;
}

export default function EmployeeFormDialog({
  open,
  employee,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!employee;
  const [employeeNumber, setEmployeeNumber] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [hireDate, setHireDate] = useState("");
  const [dateOfBirth, setDateOfBirth] = useState("");
  const [terminationDate, setTerminationDate] = useState("");
  const [address, setAddress] = useState("");
  const [city, setCity] = useState("");
  const [state, setState] = useState("");
  const [postalCode, setPostalCode] = useState("");
  const [country, setCountry] = useState("");
  const [departmentId, setDepartmentId] = useState<number | "">("");
  const [positionId, setPositionId] = useState<number | "">("");
  const [managerId, setManagerId] = useState<number | "">("");
  const [departments, setDepartments] = useState<DepartmentReadDto[]>([]);
  const [positions, setPositions] = useState<PositionReadDto[]>([]);
  const [managers, setManagers] = useState<EmployeeReadDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setEmployeeNumber(employee?.employeeNumber ?? "");
      setFirstName(employee?.firstName ?? "");
      setLastName(employee?.lastName ?? "");
      setEmail(employee?.email ?? "");
      setPhone(employee?.phone ?? "");
      setHireDate(employee?.hireDate ?? "");
      setDateOfBirth(employee?.dateOfBirth ?? "");
      setTerminationDate(employee?.terminationDate ?? "");
      setAddress(employee?.address ?? "");
      setCity(employee?.city ?? "");
      setState(employee?.state ?? "");
      setPostalCode(employee?.postalCode ?? "");
      setCountry(employee?.country ?? "");
      setDepartmentId(employee?.departmentId ?? "");
      setPositionId(employee?.positionId ?? "");
      setManagerId(employee?.managerId ?? "");
      setError(null);
      getDepartments().then(setDepartments).catch(() => {});
      getPositions().then(setPositions).catch(() => {});
      getEmployees()
        .then((list) => setManagers(list.filter((e) => e.id !== employee?.id)))
        .catch(() => {});
    }
  }, [open, employee]);

  const handleSave = async () => {
    if (saving) return;
    if (!employeeNumber.trim() || !firstName.trim() || !lastName.trim() || !email.trim() || !hireDate || departmentId === "" || positionId === "") return;
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) {
      setError("Invalid email format.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const base = {
        employeeNumber: employeeNumber.trim(),
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        phone: phone.trim() || undefined,
        dateOfBirth: dateOfBirth || undefined,
        hireDate,
        terminationDate: terminationDate || undefined,
        address: address.trim() || undefined,
        city: city.trim() || undefined,
        state: state.trim() || undefined,
        postalCode: postalCode.trim() || undefined,
        country: country.trim() || undefined,
        departmentId: departmentId as number,
        positionId: positionId as number,
        managerId: managerId === "" ? undefined : (managerId as number),
      };

      if (isEdit) {
        await updateEmployee(employee!.id, { ...base, isActive: employee!.isActive });
      } else {
        await createEmployee(base);
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
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>{isEdit ? "Edit Employee" : "Add Employee"}</DialogTitle>
      <DialogContent>
        <TextField
          label="Employee Number"
          value={employeeNumber}
          onChange={(e) => setEmployeeNumber(e.target.value)}
          fullWidth
          required
          margin="normal"
          autoFocus
        />
        <TextField
          label="First Name"
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
          fullWidth
          required
          margin="normal"
        />
        <TextField
          label="Last Name"
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
          fullWidth
          required
          margin="normal"
        />
        <TextField
          label="Email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          fullWidth
          required
          margin="normal"
        />
        <TextField
          label="Phone"
          value={phone}
          onChange={(e) => setPhone(e.target.value)}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Hire Date"
          type="date"
          value={hireDate}
          onChange={(e) => setHireDate(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Date of Birth"
          type="date"
          value={dateOfBirth}
          onChange={(e) => setDateOfBirth(e.target.value)}
          fullWidth
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Termination Date"
          type="date"
          value={terminationDate}
          onChange={(e) => setTerminationDate(e.target.value)}
          fullWidth
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          select
          label="Department"
          value={departmentId}
          onChange={(e) =>
            setDepartmentId(e.target.value === "" ? "" : Number(e.target.value))
          }
          fullWidth
          required
          margin="normal"
        >
          <MenuItem value="" disabled>Select department</MenuItem>
          {departments.map((d) => (
            <MenuItem key={d.id} value={d.id}>{d.name}</MenuItem>
          ))}
        </TextField>
        <TextField
          select
          label="Position"
          value={positionId}
          onChange={(e) =>
            setPositionId(e.target.value === "" ? "" : Number(e.target.value))
          }
          fullWidth
          required
          margin="normal"
        >
          <MenuItem value="" disabled>Select position</MenuItem>
          {positions.filter((p) => p.isActive).map((p) => (
            <MenuItem key={p.id} value={p.id}>{p.title}</MenuItem>
          ))}
        </TextField>
        <TextField
          select
          label="Manager"
          value={managerId}
          onChange={(e) =>
            setManagerId(e.target.value === "" ? "" : Number(e.target.value))
          }
          fullWidth
          margin="normal"
        >
          <MenuItem value="">None</MenuItem>
          {managers.map((m) => (
            <MenuItem key={m.id} value={m.id}>{m.fullName}</MenuItem>
          ))}
        </TextField>
        <TextField
          label="Address"
          value={address}
          onChange={(e) => setAddress(e.target.value)}
          fullWidth
          margin="normal"
        />
        <TextField
          label="City"
          value={city}
          onChange={(e) => setCity(e.target.value)}
          fullWidth
          margin="normal"
        />
        <TextField
          label="State"
          value={state}
          onChange={(e) => setState(e.target.value)}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Postal Code"
          value={postalCode}
          onChange={(e) => setPostalCode(e.target.value)}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Country"
          value={country}
          onChange={(e) => setCountry(e.target.value)}
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
          disabled={
            !employeeNumber.trim() ||
            !firstName.trim() ||
            !lastName.trim() ||
            !email.trim() ||
            !hireDate ||
            departmentId === "" ||
            positionId === "" ||
            saving
          }
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
