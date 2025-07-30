// import { Header, List, ListItem } from 'semantic-ui-react';
import { Box, Container } from '@mui/material';
import NavBar from './NavBar';
import CssBaseline from '@mui/material/CssBaseline';
import ActivityDashboard from '../../features/activities/dashboard/ActivitiesDashboard';
import { Outlet, useLocation } from 'react-router';
import HomePage from '../../features/home/HomePage';


function App() {
  const location=useLocation();


  return (
    <Box sx={{bgcolor:'#eeeeee', minHeight:'100vh'}} >
      <CssBaseline />
      {location.pathname==='/'? <HomePage/>:(
        <>
        <NavBar />
        <Container maxWidth='xl' sx={{mt:3}}>
          <Outlet />
        </Container>
        </>  
      )
      }
      
    </Box>
  )
}

export default App
