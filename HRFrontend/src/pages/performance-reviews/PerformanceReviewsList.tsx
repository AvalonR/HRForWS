import { useState, useEffect, useCallback } from "react";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import AddIcon from "@mui/icons-material/Add";
import {
  getPerformanceReviews,
  deletePerformanceReview,
} from "../../services/PerformanceReviewService";
import type { PerformanceReviewReadDto } from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";
import PerformanceReviewFormDialog from "./PerformanceReviewFormDialog";
import DeleteConfirmDialog from "../departments/DeleteConfirmDialog";

const REVIEW_LABELS = ["Draft", "Pending", "Completed", "Cancelled"];

function statusColor(status: number) {
  switch (REVIEW_LABELS[status]) {
    case "Completed":
      return { bg: "#e8f5e9", color: "#2e7d32" };
    case "Pending":
      return { bg: "#fff3e0", color: "#e65100" };
    case "Draft":
      return { bg: "#e3f2fd", color: "#1565c0" };
    default:
      return { bg: "#f5f5f5", color: "#616161" };
  }
}

export default function PerformanceReviewsList() {
  const { user } = useAuth();
  const canManage = user?.roles.some((r) => r === "Admin" || r === "HRManager");
  const canDelete = user?.roles.includes("Admin");
  const [reviews, setReviews] = useState<PerformanceReviewReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingReview, setEditingReview] = useState<PerformanceReviewReadDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<PerformanceReviewReadDto | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchReviews = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getPerformanceReviews();
      setReviews(data);
    } catch {
      setError("Failed to load performance reviews.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchReviews();
  }, [fetchReviews]);

  const handleAdd = () => {
    setEditingReview(null);
    setFormOpen(true);
  };

  const handleEdit = (r: PerformanceReviewReadDto) => {
    setEditingReview(r);
    setFormOpen(true);
  };

  const handleFormSaved = () => {
    setFormOpen(false);
    setEditingReview(null);
    fetchReviews();
    setSnackbar({
      message: editingReview
        ? "Performance review updated successfully"
        : "Performance review created successfully",
      severity: "success",
    });
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deletePerformanceReview(deleteTarget.id);
      setDeleteTarget(null);
      fetchReviews();
      setSnackbar({ message: "Performance review deleted successfully", severity: "success" });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete performance review.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const columns: GridColDef<PerformanceReviewReadDto>[] = [
    { field: "employeeName", headerName: "Employee", width: 160 },
    { field: "reviewerName", headerName: "Reviewer", width: 160 },
    { field: "reviewDate", headerName: "Review Date", width: 110 },
    {
      field: "rating",
      headerName: "Rating",
      width: 80,
      renderCell: ({ row }) =>
        row.rating != null ? (
          <Typography sx={{ fontWeight: 600 }}>{row.rating} / 5</Typography>
        ) : (
          <Typography color="text.secondary">-</Typography>
        ),
    },
    {
      field: "status",
      headerName: "Status",
      width: 110,
      renderCell: ({ row }) => (
        <Box
          sx={{
            px: 1,
            py: 0.25,
            borderRadius: 1,
            fontSize: 12,
            fontWeight: 600,
            ...statusColor(row.status),
          }}
        >
          {REVIEW_LABELS[row.status] ?? row.status}
        </Box>
      ),
    },
    { field: "nextReviewDate", headerName: "Next Review", width: 110 },
    {
      field: "actions",
      headerName: "Actions",
      width: 100,
      sortable: false,
      renderCell: ({ row }) => (
        <Box>
          {canManage && (
            <Tooltip title="Edit">
              <IconButton size="small" onClick={() => handleEdit(row)}>
                <EditIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          )}
          {canDelete && (
            <Tooltip title="Delete">
              <IconButton size="small" color="error" onClick={() => setDeleteTarget(row)}>
                <DeleteIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          )}
        </Box>
      ),
    },
  ];

  if (error) {
    return (
      <Box sx={{ p: 2 }}>
        <Typography color="error">{error}</Typography>
        <Button onClick={fetchReviews} sx={{ mt: 1 }}>
          Retry
        </Button>
      </Box>
    );
  }

  return (
    <Box>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 2,
        }}
      >
        <Typography variant="h4">Performance Reviews</Typography>
        {canManage && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAdd}>
            Add Review
          </Button>
        )}
      </Box>

      <DataGrid
        rows={reviews}
        columns={columns}
        loading={loading}
        autoHeight
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        getRowId={(row) => row.id}
        localeText={{ noRowsLabel: "No performance reviews found" }}
      />

      {formOpen && (
        <PerformanceReviewFormDialog
          open={formOpen}
          review={editingReview}
          onSaved={handleFormSaved}
          onClose={() => { setFormOpen(false); setEditingReview(null); }}
        />
      )}

      {deleteTarget && (
        <DeleteConfirmDialog
          open={!!deleteTarget}
          title="Delete Performance Review"
          message={`Are you sure you want to delete this performance review?`}
          onConfirm={handleDelete}
          onCancel={() => setDeleteTarget(null)}
        />
      )}

      {snackbar && (
        <Snackbar
          open
          autoHideDuration={4000}
          onClose={() => setSnackbar(null)}
          anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
        >
          <Alert
            severity={snackbar.severity}
            onClose={() => setSnackbar(null)}
          >
            {snackbar.message}
          </Alert>
        </Snackbar>
      )}
    </Box>
  );
}
