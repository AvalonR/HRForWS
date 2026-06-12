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
  getEmployees,
  deleteEmployee,
} from "../../services/EmployeeService";
import type { EmployeeReadDto } from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";
import EmployeeFormDialog from "./EmployeeFormDialog";
import DeleteConfirmDialog from "../departments/DeleteConfirmDialog";

export default function EmployeesList() {
  const { user } = useAuth();
  const canCreate = user?.roles.some((r) => r === "Admin" || r === "HRManager");
  const canEdit = canCreate;
  const canDelete = user?.roles.includes("Admin");
  const [employees, setEmployees] = useState<EmployeeReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingEmp, setEditingEmp] = useState<EmployeeReadDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<EmployeeReadDto | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchEmployees = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getEmployees();
      setEmployees(data);
    } catch {
      setError("Failed to load employees.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchEmployees();
  }, [fetchEmployees]);

  const handleAdd = () => {
    setEditingEmp(null);
    setFormOpen(true);
  };

  const handleEdit = (emp: EmployeeReadDto) => {
    setEditingEmp(emp);
    setFormOpen(true);
  };

  const handleFormSaved = () => {
    setFormOpen(false);
    setEditingEmp(null);
    fetchEmployees();
    setSnackbar({
      message: editingEmp
        ? "Employee updated successfully"
        : "Employee created successfully",
      severity: "success",
    });
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deleteEmployee(deleteTarget.id);
      setDeleteTarget(null);
      fetchEmployees();
      setSnackbar({
        message: "Employee deleted successfully",
        severity: "success",
      });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete employee.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const columns: GridColDef<EmployeeReadDto>[] = [
    { field: "employeeNumber", headerName: "Emp #", width: 100 },
    { field: "fullName", headerName: "Name", flex: 1, minWidth: 180 },
    { field: "email", headerName: "Email", flex: 1, minWidth: 200 },
    { field: "departmentName", headerName: "Department", width: 160 },
    { field: "positionTitle", headerName: "Position", width: 160 },
    { field: "phone", headerName: "Phone", width: 130 },
    { field: "hireDate", headerName: "Hire Date", width: 110 },
    {
      field: "isActive",
      headerName: "Status",
      width: 100,
      renderCell: ({ row }) => (
        <Box
          sx={{
            px: 1,
            py: 0.25,
            borderRadius: 1,
            fontSize: 12,
            fontWeight: 600,
            bgcolor: row.isActive ? "#e8f5e9" : "#fce4ec",
            color: row.isActive ? "#2e7d32" : "#c62828",
          }}
        >
          {row.isActive ? "Active" : "Inactive"}
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
          {canEdit && (
            <Tooltip title="Edit">
              <IconButton size="small" onClick={() => handleEdit(row)}>
                <EditIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          )}
          {canDelete && (
            <Tooltip title="Delete">
              <IconButton
                size="small"
                color="error"
                onClick={() => setDeleteTarget(row)}
              >
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
        <Button onClick={fetchEmployees} sx={{ mt: 1 }}>
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
        <Typography variant="h4">Employees</Typography>
        {canCreate && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAdd}>
            Add Employee
          </Button>
        )}
      </Box>

      <DataGrid
        rows={employees}
        columns={columns}
        loading={loading}
        autoHeight
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        getRowId={(row) => row.id}
        localeText={{ noRowsLabel: "No employees found" }}
      />

      {formOpen && (
        <EmployeeFormDialog
          open={formOpen}
          employee={editingEmp}
          onSaved={handleFormSaved}
          onClose={() => {
            setFormOpen(false);
            setEditingEmp(null);
          }}
        />
      )}

      {deleteTarget && (
        <DeleteConfirmDialog
          open={!!deleteTarget}
          title="Delete Employee"
          message={`Are you sure you want to delete "${deleteTarget.fullName}"?`}
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
