import { useEffect, useState } from 'react';
import axios from 'axios';
// import { Header, List, ListItem } from 'semantic-ui-react';
import { Box, Container } from '@mui/material';
import NavBar from './NavBar';
import CssBaseline from '@mui/material/CssBaseline';
import ActivityDashboard from '../../features/activities/dashboard/ActivitiesDashboard';


function App() {
  const [activities, setActivities] = useState<Activity[]>([]);
  const [selectedActivity, setSelectedActivity] = useState<Activity | undefined>(undefined);
  const [editMode,setEditMode]=useState(false);

  useEffect(() => {
    axios.get<Activity[]>('https://localhost:5000/api/activities')
      .then(response => {
        setActivities(response.data)
      })

    return () => { }
  }, [])

  const handleSelectActivity=(id:string)=>{
    setSelectedActivity(activities.find(x => x.id === id));
  }

  const handleCancelSelectActivity=()=>{
    setSelectedActivity(undefined);
  }

  const handleOpenForm=(id?:string)=>{
    if (id) handleSelectActivity(id);
    else handleCancelSelectActivity();
    setEditMode(true);
  }

  const handleFormClose=()=>{
    setEditMode(false);
  }

  const handleSubmitForm=(activity:Activity)=>{
    if(activity.id)
    {
      setActivities(activities.map(x=>x.id===activity.id ? activity : x));
    }
    else{
      const newActivity={...activity, id: activities.length.toString()};
      setSelectedActivity(newActivity);
      setActivities([...activities,newActivity]);
    }
    setEditMode(false);
  }

  const handleDeleteActivity=(id:string)=>{
    setActivities(activities.filter(x=>x.id!==id))
  }

  return (
    <Box sx={{bgcolor:'#eeeeee'}} >
      <CssBaseline />
      <NavBar openForm={handleOpenForm}/>
      <Container maxWidth='xl' sx={{mt:3}}>
        <ActivityDashboard 
          activities={activities}
          selectActivity={handleSelectActivity}
          cancelSelectActivity={handleCancelSelectActivity}
          selectedActivity={selectedActivity}
          editMode={editMode}
          openForm={handleOpenForm}
          closeForm={handleFormClose}
          submitForm={handleSubmitForm}
          deleteActivity={handleDeleteActivity} />
      </Container>
    </Box>
  )
}

export default App
