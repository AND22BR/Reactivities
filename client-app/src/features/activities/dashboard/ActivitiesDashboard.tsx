import { Grid } from '@mui/material';
import ActivityList from './ActivityList';
import ActivityDetail from '../details/ActivityDetail';
import ActivityForm from '../form/ActivityForm';

type Props={
    activities: Activity[];
    selectActivity: (id: string)=> void;
    cancelSelectActivity: ()=> void;
    selectedActivity?: Activity;
    openForm: (id: string) => void;
    closeForm: ()=> void;
    editMode: boolean;
}

export default function ActivityDashboard({
    activities, 
    cancelSelectActivity,
    selectActivity,
    selectedActivity,
    openForm,
    closeForm,
    editMode}: Props) {
    return (
        <Grid container spacing={2}>
            <Grid size={7}>
                <ActivityList 
                    activities={activities}
                    selectActivity={selectActivity}
                     />    
            </Grid>
            <Grid size={5}>
                {selectedActivity && !editMode &&
                    <ActivityDetail 
                        selectedActivity={selectedActivity}
                        openForm={openForm}
                        cancelSelectActivity={cancelSelectActivity} />
                }    
                {editMode && 
                <ActivityForm closeForm={closeForm} activity={selectedActivity}/>}
            </Grid>
        </Grid>
    )
}

