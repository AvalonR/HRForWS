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
  getAttendances,
  deleteAttendance,
} from "../../services/AttendanceService";
import type { AttendanceReadDto } from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";
import AttendanceFormDialog from "./AttendanceFormDialog";
import DeleteConfirmDialog from "../departments/DeleteConfirmDialog";

const ATTENDANCE_LABELS = ["Present", "Absent", "Late", "HalfDay", "Remote", "OnLeave"];

function statusColor(status: number) {
  switch (ATTENDANCE_LABELS[status]) {
    case "Present":
      return { bg: "#e8f5e9", color: "#2e7d32" };
    case "Absent":
      return { bg: "#fce4ec", color: "#c62828" };
    case "Late":
      return { bg: "#fff3e0", color: "#e65100" };
    case "HalfDay":
      return { bg: "#fff8e1", color: "#f57f17" };
    case "Remote":
      return { bg: "#e3f2fd", color: "#1565c0" };
    case "OnLeave":
      return { bg: "#f3e5f5", color: "#7b1fa2" };
    default:
      return { bg: "#f5f5f5", color: "#616161" };
  }
}

export default function AttendanceList() {
  const { user } = useAuth();
  const canManage = user?.roles.some((r) => r === "Admin" || r === "HRManager");
  const canDelete = user?.roles.includes("Admin");
  const [attendance, setAttendance] = useState<AttendanceReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<AttendanceReadDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<AttendanceReadDto | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchAttendance = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getAttendances();
      setAttendance(data);
    } catch {
      setError("Failed to load attendance records.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchAttendance();
  }, [fetchAttendance]);

  const handleAdd = () => {
    setEditingRecord(null);
    setFormOpen(true);
  };

  const handleEdit = (r: AttendanceReadDto) => {
    setEditingRecord(r);
    setFormOpen(true);
  };

  const handleFormSaved = () => {
    setFormOpen(false);
    setEditingRecord(null);
    fetchAttendance();
    setSnackbar({
      message: editingRecord
        ? "Attendance updated successfully"
        : "Attendance created successfully",
      severity: "success",
    });
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await deleteAttendance(deleteTarget.id);
      setDeleteTarget(null);
      fetchAttendance();
      setSnackbar({ message: "Attendance deleted successfully", severity: "success" });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete attendance.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const columns: GridColDef<AttendanceReadDto>[] = [
    { field: "employeeName", headerName: "Employee", width: 160 },
    { field: "date", headerName: "Date", width: 110 },
    { field: "checkIn", headerName: "Check In", width: 100 },
    { field: "checkOut", headerName: "Check Out", width: 100 },
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
          {ATTENDANCE_LABELS[row.status] ?? row.status}
        </Box>
      ),
    },
    { field: "notes", headerName: "Notes", flex: 1, minWidth: 150 },
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
        <Button onClick={fetchAttendance} sx={{ mt: 1 }}>
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
        <Typography variant="h4">Attendance</Typography>
        {canManage && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAdd}>
            Add Attendance
          </Button>
        )}
      </Box>

      <DataGrid
        rows={attendance}
        columns={columns}
        loading={loading}
        autoHeight
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50]}
        initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
        getRowId={(row) => row.id}
        localeText={{ noRowsLabel: "No attendance records found" }}
      />

      {formOpen && (
        <AttendanceFormDialog
          open={formOpen}
          attendance={editingRecord}
          onSaved={handleFormSaved}
          onClose={() => { setFormOpen(false); setEditingRecord(null); }}
        />
      )}

      {deleteTarget && (
        <DeleteConfirmDialog
          open={!!deleteTarget}
          title="Delete Attendance Record"
          message={`Are you sure you want to delete attendance for "${deleteTarget.employeeName}" on ${deleteTarget.date}?`}
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
