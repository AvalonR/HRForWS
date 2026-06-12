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
  getPayrollRecords,
  deletePayrollRecord,
} from "../../services/PayrollRecordService";
import type { PayrollRecordReadDto } from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";
import PayrollFormDialog from "./PayrollFormDialog";
import DeleteConfirmDialog from "../departments/DeleteConfirmDialog";

const PAYROLL_LABELS = ["Pending", "Processed", "Paid", "Cancelled"];

function statusColor(status: number) {
  switch (PAYROLL_LABELS[status]) {
    case "Paid":
      return { bg: "#e8f5e9", color: "#2e7d32" };
    case "Processed":
      return { bg: "#fff3e0", color: "#e65100" };
    case "Pending":
      return { bg: "#e3f2fd", color: "#1565c0" };
    default:
      return { bg: "#f5f5f5", color: "#616161" };
  }
}

export default function PayrollList() {
  const { user } = useAuth();
  const canManage = user?.roles.some((r) => r === "Admin" || r === "HRManager");
  const canDelete = user?.roles.includes("Admin");
  const [records, setRecords] = useState<PayrollRecordReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<PayrollRecordReadDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<PayrollRecordReadDto | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchRecords = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getPayrollRecords();
      setRecords(data);
    } catch {
      setError("Failed to load payroll records.");
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

  const handleEdit = (r: PayrollRecordReadDto) => {
    setEditingRecord(r);
    setFormOpen(true);
  };

  const handleFormSaved = () => {
    setFormOpen(false);
    setEditingRecord(null);
    fetchRecords();
    setSnackbar({
      message: editingRecord
        ? "Payroll record updated successfully"
        : "Payroll record created successfully",
      severity: "success",
    });
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deletePayrollRecord(deleteTarget.id);
      setDeleteTarget(null);
      fetchRecords();
      setSnackbar({ message: "Payroll record deleted successfully", severity: "success" });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete payroll record.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const columns: GridColDef<PayrollRecordReadDto>[] = [
    { field: "employeeName", headerName: "Employee", width: 160 },
    { field: "payPeriodStart", headerName: "Period Start", width: 110 },
    { field: "payPeriodEnd", headerName: "Period End", width: 110 },
    {
      field: "baseSalary",
      headerName: "Base Salary",
      width: 120,
      valueFormatter: (value?: number) =>
        value != null ? `$${value.toLocaleString()}` : "-",
    },
    {
      field: "netPay",
      headerName: "Net Pay",
      width: 120,
      valueFormatter: (value?: number) =>
        value != null ? `$${value.toLocaleString()}` : "-",
    },
    { field: "payDate", headerName: "Pay Date", width: 110 },
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
          {PAYROLL_LABELS[row.status] ?? row.status}
        </Box>
      ),
    },
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
        <Typography variant="h4">Payroll Records</Typography>
        {canManage && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAdd}>
            Add Payroll Record
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
        localeText={{ noRowsLabel: "No payroll records found" }}
      />

      {formOpen && (
        <PayrollFormDialog
          open={formOpen}
          record={editingRecord}
          onSaved={handleFormSaved}
          onClose={() => { setFormOpen(false); setEditingRecord(null); }}
        />
      )}

      {deleteTarget && (
        <DeleteConfirmDialog
          open={!!deleteTarget}
          title="Delete Payroll Record"
          message={`Are you sure you want to delete payroll record for "${deleteTarget.employeeName}"?`}
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
