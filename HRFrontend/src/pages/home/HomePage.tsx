import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { useAuth } from "../../contexts/AuthContext";

export default function HomePage() {
  const { user } = useAuth();

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        minHeight: "60vh",
      }}
    >
      <Card sx={{ maxWidth: 500, width: "100%" }}>
        <CardContent sx={{ p: 4, textAlign: "center" }}>
          <Typography variant="h4" sx={{ mb: 1 }}>
            Welcome
          </Typography>
          <Typography variant="body1" color="text.secondary">
            {user?.email}
          </Typography>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mt: 0.5, mb: 3 }}
          >
            Roles: {user?.roles.join(", ")}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Use the sidebar to navigate between sections.
          </Typography>
        </CardContent>
      </Card>
    </Box>
  );
}
