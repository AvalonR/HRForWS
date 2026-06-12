import { Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import LoginPage from "./pages/auth/LoginPage";
import HomePage from "./pages/home/HomePage";
import DepartmentsList from "./pages/departments/DepartmentsList";
import PositionsList from "./pages/positions/PositionsList";
import EmployeesList from "./pages/employees/EmployeesList";
import LeavePage from "./pages/leave/LeavePage";
import AttendanceList from "./pages/attendance/AttendanceList";
import PayrollList from "./pages/payroll/PayrollList";
import SalaryHistoryList from "./pages/salary-history/SalaryHistoryList";
import PerformanceReviewsList from "./pages/performance-reviews/PerformanceReviewsList";

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/employees" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><EmployeesList /></ProtectedRoute>} />
          <Route path="/departments" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><DepartmentsList /></ProtectedRoute>} />
          <Route path="/positions" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><PositionsList /></ProtectedRoute>} />
          <Route path="/leave" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead", "Employee"]}><LeavePage /></ProtectedRoute>} />
          <Route path="/attendance" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><AttendanceList /></ProtectedRoute>} />
          <Route path="/payroll" element={<ProtectedRoute roles={["Admin", "HRManager"]}><PayrollList /></ProtectedRoute>} />
          <Route path="/salary-history" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><SalaryHistoryList /></ProtectedRoute>} />
          <Route path="/performance-reviews" element={<ProtectedRoute roles={["Admin", "HRManager", "TeamLead"]}><PerformanceReviewsList /></ProtectedRoute>} />
        </Route>
      </Route>
    </Routes>
  );
}

export default App;
