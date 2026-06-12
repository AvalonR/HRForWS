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
  createPerformanceReview,
  updatePerformanceReview,
} from "../../services/PerformanceReviewService";
import { getEmployees } from "../../services/EmployeeService";
import type {
  PerformanceReviewReadDto,
  PerformanceReviewUpdateDto,
  EmployeeReadDto,
} from "../../types/dto";
import { getErrorMessage } from "../../utils/errorUtils";

const STATUS_OPTIONS = [
  { value: 0, label: "Draft" },
  { value: 1, label: "Pending" },
  { value: 2, label: "Completed" },
  { value: 3, label: "Cancelled" },
];

interface Props {
  open: boolean;
  review: PerformanceReviewReadDto | null;
  onSaved: () => void;
  onClose: () => void;
}

export default function PerformanceReviewFormDialog({
  open,
  review,
  onSaved,
  onClose,
}: Props) {
  const isEdit = !!review;
  const [employeeId, setEmployeeId] = useState<number | "">("");
  const [reviewerId, setReviewerId] = useState<number | "">("");
  const [reviewDate, setReviewDate] = useState("");
  const [rating, setRating] = useState<number | "">("");
  const [status, setStatus] = useState<number>(0);
  const [strengths, setStrengths] = useState("");
  const [areasForImprovement, setAreasForImprovement] = useState("");
  const [goals, setGoals] = useState("");
  const [nextReviewDate, setNextReviewDate] = useState("");
  const [employees, setEmployees] = useState<EmployeeReadDto[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open) {
      setEmployeeId(review?.employeeId ?? "");
      setReviewerId(review?.reviewerId ?? "");
      setReviewDate(review?.reviewDate ?? "");
      setRating(review?.rating ?? "");
      setStatus(review?.status ?? 0);
      setStrengths(review?.strengths ?? "");
      setAreasForImprovement(review?.areasForImprovement ?? "");
      setGoals(review?.goals ?? "");
      setNextReviewDate(review?.nextReviewDate ?? "");
      setError(null);
      getEmployees()
        .then((list) => setEmployees(list))
        .catch(() => {});
    }
  }, [open, review]);

  const handleSave = async () => {
    if (saving) return;
    if (employeeId === "" || reviewerId === "" || !reviewDate) return;

    setSaving(true);
    setError(null);

    try {
      const dto = {
        employeeId: employeeId as number,
        reviewerId: reviewerId as number,
        reviewDate,
        rating: rating !== "" ? rating : undefined,
        status,
        strengths: strengths.trim() || undefined,
        areasForImprovement: areasForImprovement.trim() || undefined,
        goals: goals.trim() || undefined,
        nextReviewDate: nextReviewDate || undefined,
      };

      if (isEdit) {
        await updatePerformanceReview(review!.id, dto as PerformanceReviewUpdateDto);
      } else {
        await createPerformanceReview(dto);
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
      <DialogTitle>{isEdit ? "Edit Performance Review" : "Add Performance Review"}</DialogTitle>
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
          select
          label="Reviewer"
          value={reviewerId}
          onChange={(e) =>
            setReviewerId(e.target.value === "" ? "" : Number(e.target.value))
          }
          fullWidth
          required
          margin="normal"
        >
          <MenuItem value="" disabled>Select reviewer</MenuItem>
          {employees.map((e) => (
            <MenuItem key={e.id} value={e.id}>{e.fullName}</MenuItem>
          ))}
        </TextField>
        <TextField
          label="Review Date"
          type="date"
          value={reviewDate}
          onChange={(e) => setReviewDate(e.target.value)}
          fullWidth
          required
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          select
          label="Rating"
          value={rating}
          onChange={(e) => setRating(e.target.value === "" ? "" : Number(e.target.value))}
          fullWidth
          margin="normal"
        >
          <MenuItem value="">Not rated</MenuItem>
          {[1, 2, 3, 4, 5].map((r) => (
            <MenuItem key={r} value={r}>{r} / 5</MenuItem>
          ))}
        </TextField>
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
          label="Strengths"
          value={strengths}
          onChange={(e) => setStrengths(e.target.value)}
          fullWidth
          multiline
          rows={2}
          margin="normal"
        />
        <TextField
          label="Areas for Improvement"
          value={areasForImprovement}
          onChange={(e) => setAreasForImprovement(e.target.value)}
          fullWidth
          multiline
          rows={2}
          margin="normal"
        />
        <TextField
          label="Goals"
          value={goals}
          onChange={(e) => setGoals(e.target.value)}
          fullWidth
          multiline
          rows={2}
          margin="normal"
        />
        <TextField
          label="Next Review Date"
          type="date"
          value={nextReviewDate}
          onChange={(e) => setNextReviewDate(e.target.value)}
          fullWidth
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
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
          disabled={employeeId === "" || reviewerId === "" || !reviewDate || saving}
        >
          {saving ? <CircularProgress size={20} /> : isEdit ? "Save" : "Create"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
