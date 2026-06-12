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
  getSalaryHistories,
  deleteSalaryHistory,
} from "../../services/SalaryHistoryService";
import type { SalaryHistoryReadDto } from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";
import SalaryHistoryFormDialog from "./SalaryHistoryFormDialog";
import DeleteConfirmDialog from "../departments/DeleteConfirmDialog";

export default function SalaryHistoryList() {
  const { user } = useAuth();
  const canManage = user?.roles.some((r) => r === "Admin" || r === "HRManager");
  const canDelete = user?.roles.includes("Admin");
  const [records, setRecords] = useState<SalaryHistoryReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<SalaryHistoryReadDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<SalaryHistoryReadDto | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchRecords = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getSalaryHistories();
      setRecords(data);
    } catch {
      setError("Failed to load salary histories.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchRecords();
  }, [fetchRecords]);

  const handleAdd = () => {
    setEditingRecord(null);
    setFormOpen(true);
  };

  const handleEdit = (r: SalaryHistoryReadDto) => {
    setEditingRecord(r);
    setFormOpen(true);
  };

  const handleFormSaved = () => {
    setFormOpen(false);
    setEditingRecord(null);
    fetchRecords();
    setSnackbar({
      message: editingRecord
        ? "Salary history updated successfully"
        : "Salary history created successfully",
      severity: "success",
    });
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deleteSalaryHistory(deleteTarget.id);
      setDeleteTarget(null);
      fetchRecords();
      setSnackbar({ message: "Salary history deleted successfully", severity: "success" });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete salary history.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const columns: GridColDef<SalaryHistoryReadDto>[] = [
    { field: "employeeName", headerName: "Employee", width: 160 },
    {
      field: "amount",
      headerName: "Amount",
      width: 130,
      valueFormatter: (value?: number) =>
        value != null ? `$${value.toLocaleString()}` : "-",
    },
    { field: "effectiveFrom", headerName: "Effective From", width: 130 },
    { field: "effectiveTo", headerName: "Effective To", width: 130 },
    { field: "changeReason", headerName: "Reason", flex: 1, minWidth: 200 },
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
        <Button onClick={fetchRecords} sx={{ mt: 1 }}>
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
        <Typography variant="h4">Salary History</Typography>
        {canManage && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAdd}>
            Add Salary History
          </Button>
        )}
      </Box>

      <DataGrid
        rows={records}
        columns={columns}
        loading={loading}
        autoHeight
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        getRowId={(row) => row.id}
        localeText={{ noRowsLabel: "No salary history found" }}
      />

      {formOpen && (
        <SalaryHistoryFormDialog
          open={formOpen}
          record={editingRecord}
          onSaved={handleFormSaved}
          onClose={() => { setFormOpen(false); setEditingRecord(null); }}
        />
      )}

      {deleteTarget && (
        <DeleteConfirmDialog
          open={!!deleteTarget}
          title="Delete Salary History"
          message={`Are you sure you want to delete this salary history record?`}
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
