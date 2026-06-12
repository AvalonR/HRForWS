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
  createPosition,
  updatePosition,
} from "../../services/PositionService";
import { getDepartments } from "../../services/DepartmentService";
import type {
  PositionReadDto,
  PositionCreateDto,
  PositionUpdateDto,
  DepartmentReadDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

interface Props {
  open: boolean;
  position: PositionReadDto | null;
  onSaved: () => void;
  onClose: () => void;
}

export default function PositionFormDialog({
  open,
  position,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!position;
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [departmentId, setDepartmentId] = useState<number | "">("");
  const [minSalary, setMinSalary] = useState<string>("");
  const [maxSalary, setMaxSalary] = useState<string>("");
  const [departments, setDepartments] = useState<DepartmentReadDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setTitle(position?.title ?? "");
      setDescription(position?.description ?? "");
      setDepartmentId(position?.departmentId ?? "");
      setMinSalary(position?.minSalary != null ? String(position.minSalary) : "");
      setMaxSalary(position?.maxSalary != null ? String(position.maxSalary) : "");
      setError(null);
      getDepartments()
        .then(setDepartments)
        .catch(() => {});
    }
  }, [open, position]);

  const handleSave = async () => {
    if (saving) return;
    if (!title.trim() || departmentId === "") return;

    setSaving(true);
    setError(null);

    try {
      if (isEdit) {
        const dto: PositionUpdateDto = {
          title: title.trim(),
          description: description.trim() || undefined,
          departmentId: departmentId as number,
          minSalary: minSalary ? Number(minSalary) : undefined,
          maxSalary: maxSalary ? Number(maxSalary) : undefined,
          isActive: position!.isActive,
        };
        await updatePosition(position!.id, dto);
      } else {
        const dto: PositionCreateDto = {
          title: title.trim(),
          description: description.trim() || undefined,
          departmentId: departmentId as number,
          minSalary: minSalary ? Number(minSalary) : undefined,
          maxSalary: maxSalary ? Number(maxSalary) : undefined,
        };
        await createPosition(dto);
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
      <DialogTitle>{isEdit ? "Edit Position" : "Add Position"}</DialogTitle>
      <DialogContent>
        <TextField
          label="Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          fullWidth
          required
          margin="normal"
          autoFocus
        />
        <TextField
          select
          label="Department"
          value={departmentId}
          onChange={(e) =>
            setDepartmentId(
              e.target.value === "" ? "" : Number(e.target.value),
            )
          }
          fullWidth
          required
          margin="normal"
        >
          <MenuItem value="" disabled>
            Select department
          </MenuItem>
          {departments.map((d) => (
            <MenuItem key={d.id} value={d.id}>
              {d.name}
            </MenuItem>
          ))}
        </TextField>
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
          label="Min Salary"
          value={minSalary}
          onChange={(e) => setMinSalary(e.target.value)}
          type="number"
          fullWidth
          margin="normal"
        />
        <TextField
          label="Max Salary"
          value={maxSalary}
          onChange={(e) => setMaxSalary(e.target.value)}
          type="number"
          fullWidth
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
          disabled={!title.trim() || departmentId === "" || saving}
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
