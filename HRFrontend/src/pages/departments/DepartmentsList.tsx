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
  getDepartments,
  deleteDepartment,
} from "../../services/DepartmentService";
import type { DepartmentReadDto } from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import DepartmentFormDialog from "./DepartmentFormDialog";
import DeleteConfirmDialog from "./DeleteConfirmDialog";

export default function DepartmentsList() {
  const { user } = useAuth();
  const canCreate = user?.roles.some((r) => r === "Admin" || r === "HRManager");
  const canEdit = canCreate;
  const canDelete = user?.roles.includes("Admin");
  const [departments, setDepartments] = useState<DepartmentReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingDept, setEditingDept] = useState<DepartmentReadDto | null>(
    null,
  );
  const [deleteTarget, setDeleteTarget] = useState<DepartmentReadDto | null>(
    null,
  );
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchDepartments = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getDepartments();
      setDepartments(data);
    } catch {
      setError("Failed to load departments.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchDepartments();
  }, [fetchDepartments]);

  const handleAdd = () => {
    setEditingDept(null);
    setFormOpen(true);
  };

  const handleEdit = (dept: DepartmentReadDto) => {
    setEditingDept(dept);
    setFormOpen(true);
  };

  const handleFormSaved = () => {
    setFormOpen(false);
    setEditingDept(null);
    fetchDepartments();
    setSnackbar({
      message: editingDept
        ? "Department updated successfully"
        : "Department created successfully",
      severity: "success",
    });
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deleteDepartment(deleteTarget.id);
      setDeleteTarget(null);
      fetchDepartments();
      setSnackbar({
        message: "Department deleted successfully",
        severity: "success",
      });
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: string } })?.response?.data ||
        "Failed to delete department.";
      setSnackbar({ message, severity: "error" });
    }
  };

  const columns: GridColDef<DepartmentReadDto>[] = [
    { field: "code", headerName: "Code", width: 100 },
    { field: "name", headerName: "Name", flex: 1, minWidth: 200 },
    {
      field: "description",
      headerName: "Description",
      flex: 1,
      minWidth: 200,
    },
    {
      field: "parentDepartmentId",
      headerName: "Parent Department",
      width: 180,
      valueGetter: (_, row) => {
        if (!row.parentDepartmentId) return null;
        const parent = departments.find(
          (d) => d.id === row.parentDepartmentId,
        );
        return parent?.name ?? null;
      },
    },
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
        <Button onClick={fetchDepartments} sx={{ mt: 1 }}>
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
        <Typography variant="h4">Departments</Typography>
        {canCreate && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAdd}>
            Add Department
          </Button>
        )}
      </Box>

      <DataGrid
        rows={departments}
        columns={columns}
        loading={loading}
        autoHeight
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        getRowId={(row) => row.id}
        localeText={{ noRowsLabel: "No departments found" }}
      />

      {formOpen && (
        <DepartmentFormDialog
          open={formOpen}
          department={editingDept}
          departments={departments}
          onSaved={handleFormSaved}
          onClose={() => {
            setFormOpen(false);
            setEditingDept(null);
          }}
        />
      )}

      {deleteTarget && (
        <DeleteConfirmDialog
          open={!!deleteTarget}
          title="Delete Department"
          message={`Are you sure you want to delete "${deleteTarget.name}"?`}
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
