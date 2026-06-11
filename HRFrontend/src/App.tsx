import { Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import LoginPage from "./pages/auth/LoginPage";
import DepartmentsList from "./pages/departments/DepartmentsList";

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/" element={<div>Home</div>} />
          <Route path="/employees" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><div>Employees</div></ProtectedRoute>} />
          <Route path="/departments" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><DepartmentsList /></ProtectedRoute>} />
          <Route path="/positions" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><div>Positions</div></ProtectedRoute>} />
          <Route path="/leave" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead", "Employee"]}><div>Leave</div></ProtectedRoute>} />
        </Route>
      </Route>
    </Routes>
  );
}

export default App;
