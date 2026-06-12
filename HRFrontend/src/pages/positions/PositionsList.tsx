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
  getPositions,
  deletePosition,
} from "../../services/PositionService";
import type { PositionReadDto } from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";
import PositionFormDialog from "./PositionFormDialog";
import DeleteConfirmDialog from "../departments/DeleteConfirmDialog";

export default function PositionsList() {
  const { user } = useAuth();
  const canCreate = user?.roles.some((r) => r === "Admin" || r === "HRManager");
  const canEdit = canCreate;
  const canDelete = user?.roles.includes("Admin");
  const [positions, setPositions] = useState<PositionReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingPos, setEditingPos] = useState<PositionReadDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<PositionReadDto | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchPositions = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getPositions();
      setPositions(data);
    } catch {
      setError("Failed to load positions.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchPositions();
  }, [fetchPositions]);

  const handleAdd = () => {
    setEditingPos(null);
    setFormOpen(true);
  };

  const handleEdit = (pos: PositionReadDto) => {
    setEditingPos(pos);
    setFormOpen(true);
  };

  const handleFormSaved = () => {
    setFormOpen(false);
    setEditingPos(null);
    fetchPositions();
    setSnackbar({
      message: editingPos
        ? "Position updated successfully"
        : "Position created successfully",
      severity: "success",
    });
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deletePosition(deleteTarget.id);
      setDeleteTarget(null);
      fetchPositions();
      setSnackbar({
        message: "Position deleted successfully",
        severity: "success",
      });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete position.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const columns: GridColDef<PositionReadDto>[] = [
    { field: "title", headerName: "Title", flex: 1, minWidth: 200 },
    {
      field: "description",
      headerName: "Description",
      flex: 1,
      minWidth: 200,
    },
    {
      field: "departmentName",
      headerName: "Department",
      width: 160,
    },
    {
      field: "minSalary",
      headerName: "Min Salary",
      width: 120,
      valueFormatter: (value?: number | null) =>
        value != null ? `$${value.toLocaleString()}` : "-",
    },
    {
      field: "maxSalary",
      headerName: "Max Salary",
      width: 120,
      valueFormatter: (value?: number | null) =>
        value != null ? `$${value.toLocaleString()}` : "-",
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
        <Button onClick={fetchPositions} sx={{ mt: 1 }}>
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
        <Typography variant="h4">Positions</Typography>
        {canCreate && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAdd}>
            Add Position
          </Button>
        )}
      </Box>

      <DataGrid
        rows={positions}
        columns={columns}
        loading={loading}
        autoHeight
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        getRowId={(row) => row.id}
        localeText={{ noRowsLabel: "No positions found" }}
      />

      {formOpen && (
        <PositionFormDialog
          open={formOpen}
          position={editingPos}
          onSaved={handleFormSaved}
          onClose={() => {
            setFormOpen(false);
            setEditingPos(null);
          }}
        />
      )}

      {deleteTarget && (
        <DeleteConfirmDialog
          open={!!deleteTarget}
          title="Delete Position"
          message={`Are you sure you want to delete "${deleteTarget.title}"?`}
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
