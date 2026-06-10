import { Routes, Route } from 'react-router-dom'
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'

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
        <Route path="/" />
      </Routes>
    </Box>
  )
}

export default App
