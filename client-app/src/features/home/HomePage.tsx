import { Box, Button, Paper, Typography } from "@mui/material";
import { Link } from "react-router";

export default function HomePage() {
  return (
    <Paper sx={{ 
      color: 'white',
      display: 'flex',
      flexDirection: 'column',
      alignContent: 'center',
      alignItems: 'center',
      justifyContent: 'center',
      gap:6,
      height: '100vh',
      backgroundImage: 'linear-gradient(135deg,#182a73 0%, #218aae 69%, #20a7ac 89%)',
    }}>
      <Box sx={{
        display: 'flex', 
        alignContent: 'center', 
        flexDirection: 'column', 
        gap: 3,
        color: 'white'
      }}>

      </Box>
      <Typography variant="h2">
        Welcome to Reactivities
      </Typography>
      <Button 
        component={Link} 
        to='/activities' 
        size="large"
        variant="contained" 
        sx={{ height: 80, borderRadius: 4, fontSize: '1.5rem' }}>
          Take me to the activities!
        </Button>
      </Paper>
  )
}
