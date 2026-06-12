import { useState, type ReactNode } from "react";
import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Drawer from "@mui/material/Drawer";
import List from "@mui/material/List";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import Box from "@mui/material/Box";
import IconButton from "@mui/material/IconButton";
import Avatar from "@mui/material/Avatar";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import Divider from "@mui/material/Divider";
import BusinessIcon from "@mui/icons-material/Business";
import GroupIcon from "@mui/icons-material/Group";
import WorkIcon from "@mui/icons-material/Work";
import EventNoteIcon from "@mui/icons-material/EventNote";
import CalendarMonthIcon from "@mui/icons-material/CalendarMonth";
import AttachMoneyIcon from "@mui/icons-material/AttachMoney";
import TrendingUpIcon from "@mui/icons-material/TrendingUp";
import RateReviewIcon from "@mui/icons-material/RateReview";

const DRAWER_WIDTH = 240;

interface NavItem {
  label: string;
  path: string;
  icon: ReactNode;
  roles: string[];
}

const allNavItems: NavItem[] = [
  { label: "Departments", path: "/departments", icon: <BusinessIcon />, roles: ["Admin", "HRManager", "TeamLead"] },
  { label: "Positions", path: "/positions", icon: <WorkIcon />, roles: ["Admin", "HRManager", "TeamLead"] },
  { label: "Employees", path: "/employees", icon: <GroupIcon />, roles: ["Admin", "HRManager", "TeamLead"] },
  { label: "Leave", path: "/leave", icon: <EventNoteIcon />, roles: ["Admin", "HRManager", "TeamLead", "Employee"] },
  { label: "Attendance", path: "/attendance", icon: <CalendarMonthIcon />, roles: ["Admin", "HRManager", "TeamLead"] },
  { label: "Payroll", path: "/payroll", icon: <AttachMoneyIcon />, roles: ["Admin", "HRManager"] },
  { label: "Salary History", path: "/salary-history", icon: <TrendingUpIcon />, roles: ["Admin", "HRManager", "TeamLead"] },
  { label: "Performance Reviews", path: "/performance-reviews", icon: <RateReviewIcon />, roles: ["Admin", "HRManager", "TeamLead"] },
];

export default function MainLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);

  const visibleItems = allNavItems.filter(
    (item) => user && item.roles.some((r) => user.roles.includes(r))
  );

  const initials = user?.email ? user.email[0].toUpperCase() : "?";

  return (
    <Box sx={{ display: "flex" }}>
      <AppBar position="fixed" sx={{ zIndex: (t) => t.zIndex.drawer + 1 }}>
        <Toolbar>
          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            HR Management System
          </Typography>
          <IconButton color="inherit" onClick={(e) => setAnchorEl(e.currentTarget)}>
            <Avatar sx={{ width: 32, height: 32, bgcolor: "secondary.main", fontSize: 16 }}>
              {initials}
            </Avatar>
          </IconButton>
          <Menu
            anchorEl={anchorEl}
            open={!!anchorEl}
            onClose={() => setAnchorEl(null)}
            anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
            transformOrigin={{ vertical: "top", horizontal: "right" }}
          >
            <MenuItem disabled sx={{ opacity: 1 }}>
              <Box sx={{ display: "flex", flexDirection: "column" }}>
                <Typography variant="body2" sx={{ fontWeight: 600 }}>{user?.email}</Typography>
                <Typography variant="caption" color="text.secondary">
                  {user?.roles.join(", ")}
                </Typography>
              </Box>
            </MenuItem>
            <Divider />
            <MenuItem onClick={() => { setAnchorEl(null); logout(); }}>
              Logout
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="permanent"
        sx={{
          width: DRAWER_WIDTH,
          "& .MuiDrawer-paper": { width: DRAWER_WIDTH, boxSizing: "border-box" },
        }}
      >
        <Toolbar />
        <List>
          {visibleItems.map((item) => (
            <ListItemButton
              key={item.path}
              selected={location.pathname === item.path}
              onClick={() => navigate(item.path)}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          ))}
        </List>
      </Drawer>

      <Box
        component="main"
        sx={{ flexGrow: 1, p: 3, mt: 8 }}
      >
        <Outlet />
      </Box>
    </Box>
  );
}
