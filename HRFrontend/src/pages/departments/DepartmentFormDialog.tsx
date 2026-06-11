import { useState, useEffect } from "react";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";
import TextField from "@mui/material/TextField";
import MenuItem from "@mui/material/MenuItem";
import Button from "@mui/material/Button";
import CircularProgress from "@mui/material/CircularProgress";
import {
  createDepartment,
  updateDepartment,
} from "../../services/DepartmentService";
import type {
  DepartmentReadDto,
  DepartmentCreateDto,
  DepartmentUpdateDto,
} from "../../types/dto";

interface Props {
  open: boolean;
  department: DepartmentReadDto | null;
  departments: DepartmentReadDto[];
  onSaved: () => void;
  onClose: () => void;
}

export default function DepartmentFormDialog({
  open,
  department,
  departments,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!department;
  const [name, setName] = useState("");
  const [code, setCode] = useState("");
  const [description, setDescription] = useState("");
  const [parentDepartmentId, setParentDepartmentId] = useState<number | "">("");
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setName(department?.name ?? "");
      setCode(department?.code ?? "");
      setDescription(department?.description ?? "");
      setParentDepartmentId(department?.parentDepartmentId ?? "");
      setError(null);
    }
  }, [open, department]);

  const handleSave = async () => {
    if (!name.trim() || !code.trim()) return;

    setSaving(true);
    setError(null);

    try {
      const parentId =
        parentDepartmentId === "" ? undefined : parentDepartmentId;

      if (isEdit) {
        const dto: DepartmentUpdateDto = {
          name: name.trim(),
          code: code.trim(),
          description: description.trim() || undefined,
          parentDepartmentId: parentId,
          isActive: department!.isActive,
        };
        await updateDepartment(department!.id, dto);
      } else {
        const dto: DepartmentCreateDto = {
          name: name.trim(),
          code: code.trim(),
          description: description.trim() || undefined,
          parentDepartmentId: parentId,
        };
        await createDepartment(dto);
      }
      onSaved();
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: string } })?.response?.data ||
        "An error occurred.";
      setError(message);
    } finally {
      setSaving(false);
    }
  };

  const parentOptions = departments.filter(
    (d) => d.id !== department?.id,
  );

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? "Edit Department" : "Add Department"}</DialogTitle>
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
          label="Code"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{ htmlInput: { maxLength: 20 } }}
        />
        <TextField
          label="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          fullWidth
          multiline
          rows={3}
          margin="normal"
        />
        <TextField
          select
          label="Parent Department"
          value={parentDepartmentId}
          onChange={(e) =>
            setParentDepartmentId(
              e.target.value === "" ? "" : Number(e.target.value),
            )
          }
          fullWidth
          margin="normal"
        >
          <MenuItem value="">None</MenuItem>
          {parentOptions.map((d) => (
            <MenuItem key={d.id} value={d.id}>
              {d.name}
            </MenuItem>
          ))}
        </TextField>
        {error && (
          <DialogContent>
            <DialogActions sx={{ color: "error.main", p: 0 }}>
              {error}
            </DialogActions>
          </DialogContent>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={saving}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={handleSave}
          disabled={!name.trim() || !code.trim() || saving}
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
