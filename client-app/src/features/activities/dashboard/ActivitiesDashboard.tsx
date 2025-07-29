import { Grid } from '@mui/material';
import ActivityList from './ActivityList';

export default function ActivityDashboard() {
    

    return (
        <Grid container spacing={2}>
            <Grid size={7}>
                <ActivityList />    
            </Grid>
            <Grid size={5}>
                Activity Filters go here
            </Grid>
        </Grid>
    )
}

