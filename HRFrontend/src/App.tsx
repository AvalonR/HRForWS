import { Routes, Route } from "react-router-dom";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";
import DepartmentsList from "./pages/departments/DepartmentsList";

function App() {
  return (
    <Box sx={{ p: 4 }}>
      <Typography variant="h3" gutterBottom>
        HR Management System
      </Typography>
      <Typography variant="body1" color="text.secondary">
        Select a module from the navigation to get started.
      </Typography>
      <Routes>
        <Route path="/" element={<div>Home</div>} />
        <Route path="/employees" element={<div>Employees</div>} />
        <Route path="/departments" element={<DepartmentsList />} />
        <Route path="/positions" element={<div>Positions</div>} />
        <Route path="/attendance" element={<div>Attendance</div>} />
        <Route path="/leave-requests" element={<div>Leave Requests</div>} />
        <Route path="/leave-types" element={<div>Leave Types</div>} />
      </Routes>
    </Box>
  );
}

export default App;
