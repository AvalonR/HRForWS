import { Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import DepartmentsList from "./pages/departments/DepartmentsList";

function App() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<div>Home</div>} />
        <Route path="/employees" element={<div>Employees</div>} />
        <Route path="/departments" element={<DepartmentsList />} />
        <Route path="/positions" element={<div>Positions</div>} />
        <Route path="/attendance" element={<div>Attendance</div>} />
        <Route path="/leave-requests" element={<div>Leave Requests</div>} />
        <Route path="/leave-types" element={<div>Leave Types</div>} />
      </Route>
    </Routes>
  );
}

export default App;
