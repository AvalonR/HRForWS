import { useState, useEffect, useCallback } from "react";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import AddIcon from "@mui/icons-material/Add";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import { getLeaveRequests, deleteLeaveRequest } from "../../services/LeaveRequestService";
import { getLeaveTypes, deleteLeaveType } from "../../services/LeaveTypeService";
import type {
  LeaveRequestReadDto,
  LeaveTypeReadDto,
} from "../../types/dto";
import { useAuth } from "../../contexts/AuthContext";
import { getErrorMessage } from "../../utils/errorUtils";
import LeaveRequestDialog from "./LeaveRequestDialog";
import LeaveRequestUpdateDialog from "./LeaveRequestUpdateDialog";
import LeaveTypeFormDialog from "./LeaveTypeFormDialog";
import DeleteConfirmDialog from "../departments/DeleteConfirmDialog";

const LEAVE_LABELS = ["Pending", "Approved", "Rejected", "Cancelled"];

function statusColor(status: number) {
  switch (LEAVE_LABELS[status]) {
    case "Approved":
      return { bg: "#e8f5e9", color: "#2e7d32" };
    case "Rejected":
      return { bg: "#fce4ec", color: "#c62828" };
    case "Pending":
      return { bg: "#fff3e0", color: "#e65100" };
    default:
      return { bg: "#f5f5f5", color: "#616161" };
  }
}

export default function LeavePage() {
  const { user } = useAuth();
  const canCreate = user?.roles.some(
    (r) => r === "Admin" || r === "HRManager" || r === "Employee",
  );
  const canManage = user?.roles.some(
    (r) => r === "Admin" || r === "HRManager",
  );
  const canDelete = user?.roles.includes("Admin");
  const [tab, setTab] = useState(0);
  const [requests, setRequests] = useState<LeaveRequestReadDto[]>([]);
  const [leaveTypes, setLeaveTypes] = useState<LeaveTypeReadDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [updateDialogOpen, setUpdateDialogOpen] = useState(false);
  const [selectedRequest, setSelectedRequest] = useState<LeaveRequestReadDto | null>(null);
  const [typeFormOpen, setTypeFormOpen] = useState(false);
  const [editingType, setEditingType] = useState<LeaveTypeReadDto | null>(null);
  const [deleteTypeTarget, setDeleteTypeTarget] = useState<LeaveTypeReadDto | null>(null);
  const [deleteRequestTarget, setDeleteRequestTarget] = useState<LeaveRequestReadDto | null>(null);
  const [loadingTypes, setLoadingTypes] = useState(false);
  const [typesError, setTypesError] = useState<string | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "error";
  } | null>(null);

  const fetchRequests = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await getLeaveRequests();
      const filtered =
        user?.roles.includes("Admin") || user?.roles.includes("HRManager") || user?.roles.includes("TeamLead")
          ? data
          : data.filter((r) => r.employeeId === user?.employeeId);
      setRequests(filtered);
    } catch {
      setError("Failed to load leave requests.");
    } finally {
      setLoading(false);
    }
  }, [user]);

  const fetchTypes = useCallback(async () => {
    setLoadingTypes(true);
    setTypesError(null);
    try {
      const data = await getLeaveTypes();
      setLeaveTypes(data);
    } catch {
      setTypesError("Failed to load leave types.");
    } finally {
      setLoadingTypes(false);
    }
  }, []);

  useEffect(() => {
    fetchRequests();
    fetchTypes();
  }, [fetchRequests, fetchTypes]);

  const handleCreated = () => {
    setDialogOpen(false);
    fetchRequests();
    setSnackbar({ message: "Leave request submitted successfully", severity: "success" });
  };

  const handleRequestUpdated = () => {
    setUpdateDialogOpen(false);
    setSelectedRequest(null);
    fetchRequests();
    setSnackbar({ message: "Leave request updated", severity: "success" });
  };

  const handleDeleteRequest = async () => {
    if (!deleteRequestTarget) return;
    try {
      await deleteLeaveRequest(deleteRequestTarget.id);
      setDeleteRequestTarget(null);
      fetchRequests();
      setSnackbar({ message: "Leave request deleted successfully", severity: "success" });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete leave request.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const handleTypeSaved = () => {
    setTypeFormOpen(false);
    setEditingType(null);
    getLeaveTypes().then(setLeaveTypes).catch(() => {});
    setSnackbar({
      message: editingType
        ? "Leave type updated successfully"
        : "Leave type created successfully",
      severity: "success",
    });
  };

  const handleDeleteType = async () => {
    if (!deleteTypeTarget) return;
    try {
      await deleteLeaveType(deleteTypeTarget.id);
      setDeleteTypeTarget(null);
      fetchTypes();
      setSnackbar({ message: "Leave type deleted successfully", severity: "success" });
    } catch (err: unknown) {
      const message = getErrorMessage(err, "Failed to delete leave type.");
      setSnackbar({ message, severity: "error" });
    }
  };

  const requestColumns: GridColDef<LeaveRequestReadDto>[] = [
    { field: "employeeName", headerName: "Employee", width: 160 },
    { field: "leaveTypeName", headerName: "Leave Type", width: 130 },
    { field: "startDate", headerName: "Start Date", width: 110 },
    { field: "endDate", headerName: "End Date", width: 110 },
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
          {LEAVE_LABELS[row.status] ?? row.status}
        </Box>
      ),
    },
    { field: "reason", headerName: "Reason", flex: 1, minWidth: 150 },
    {
      field: "actions",
      headerName: "Actions",
      width: 100,
      sortable: false,
      renderCell: ({ row }) => (
        <Box>
          {(canManage || row.employeeId === user?.employeeId) && (
            <Tooltip title="Edit">
              <IconButton size="small" onClick={() => { setSelectedRequest(row); setUpdateDialogOpen(true); }}>
                <EditIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          )}
          {canDelete && (
            <Tooltip title="Delete">
              <IconButton size="small" color="error" onClick={() => setDeleteRequestTarget(row)}>
                <DeleteIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          )}
        </Box>
      ),
    },
  ];

  const typeColumns: GridColDef<LeaveTypeReadDto>[] = [
    { field: "name", headerName: "Name", flex: 1, minWidth: 200 },
    { field: "daysAllowed", headerName: "Days Allowed", width: 130 },
    {
      field: "isPaid",
      headerName: "Paid",
      width: 80,
      renderCell: ({ row }) => (
        <Typography>{row.isPaid ? "Yes" : "No"}</Typography>
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
              <IconButton size="small" onClick={() => { setEditingType(row); setTypeFormOpen(true); }}>
                <EditIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          )}
          {canDelete && (
            <Tooltip title="Delete">
              <IconButton size="small" color="error" onClick={() => setDeleteTypeTarget(row)}>
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
        <Button onClick={fetchRequests} sx={{ mt: 1 }}>
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
        <Typography variant="h4">Leave</Typography>
        {canCreate && (
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => setDialogOpen(true)}
          >
            New Request
          </Button>
        )}
      </Box>

      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 2 }}>
        <Tab label="Requests" />
        <Tab label="Leave Types" />
      </Tabs>

      {tab === 0 && (
        <DataGrid
          rows={requests}
          columns={requestColumns}
          loading={loading}
          autoHeight
          disableRowSelectionOnClick
          pageSizeOptions={[10, 25, 50]}
          initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
          getRowId={(row) => row.id}
          localeText={{ noRowsLabel: "No leave requests found" }}
        />
      )}

      {tab === 1 && (
        <Box>
          <Box sx={{ display: "flex", justifyContent: "flex-end", mb: 1 }}>
            {canManage && (
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={() => { setEditingType(null); setTypeFormOpen(true); }}
              >
                Add Leave Type
              </Button>
            )}
          </Box>
          {typesError ? (
            <Box sx={{ p: 2 }}>
              <Typography color="error">{typesError}</Typography>
              <Button onClick={fetchTypes} sx={{ mt: 1 }}>Retry</Button>
            </Box>
          ) : (
            <DataGrid
              rows={leaveTypes}
              columns={typeColumns}
              loading={loadingTypes}
              autoHeight
              disableRowSelectionOnClick
              pageSizeOptions={[10, 25, 50]}
              initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
              getRowId={(row) => row.id}
              localeText={{ noRowsLabel: "No leave types found" }}
            />
          )}
        </Box>
      )}

      {dialogOpen && (
        <LeaveRequestDialog
          open={dialogOpen}
          leaveTypes={leaveTypes}
          onCreated={handleCreated}
          onClose={() => setDialogOpen(false)}
        />
      )}

      {updateDialogOpen && selectedRequest && (
        <LeaveRequestUpdateDialog
          open={updateDialogOpen}
          request={selectedRequest}
          onSaved={handleRequestUpdated}
          onClose={() => { setUpdateDialogOpen(false); setSelectedRequest(null); }}
        />
      )}

      {typeFormOpen && (
        <LeaveTypeFormDialog
          open={typeFormOpen}
          leaveType={editingType}
          onSaved={handleTypeSaved}
          onClose={() => { setTypeFormOpen(false); setEditingType(null); }}
        />
      )}

      {deleteRequestTarget && (
        <DeleteConfirmDialog
          open={!!deleteRequestTarget}
          title="Delete Leave Request"
          message={`Are you sure you want to delete the leave request for "${deleteRequestTarget.employeeName}"?`}
          onConfirm={handleDeleteRequest}
          onCancel={() => setDeleteRequestTarget(null)}
        />
      )}

      {deleteTypeTarget && (
        <DeleteConfirmDialog
          open={!!deleteTypeTarget}
          title="Delete Leave Type"
          message={`Are you sure you want to delete "${deleteTypeTarget.name}"?`}
          onConfirm={handleDeleteType}
          onCancel={() => setDeleteTypeTarget(null)}
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
