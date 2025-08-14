import { Grid, Typography } from "@mui/material";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useParams } from "react-router";
import ActivityDetailsInfo from "./ActivityDetailsInfo";
import ActivityDetailsHeader from "./ActivityDetailsHeader";
import ActivityDetailsChat from "./ActivityDetailsChat";
import ActivityDetailsSidebar from "./ActivityDetailsSidebar";

export default function ActivityDetail() {
    const {id}=useParams();
    const {activity, isLoadingActivity}=useActivities(id);

    if(isLoadingActivity){
        return(<Typography>Loading...</Typography>)
    }

    if(!activity){
        return(<Typography>Activity Not Found</Typography>)
    }
  
    return (
        <Grid container spacing={3}>
            <Grid size={8}>
                <ActivityDetailsHeader activity={activity}/>
                <ActivityDetailsInfo activity={activity}/>
                <ActivityDetailsChat/>
            </Grid>
            <Grid size={4}>
                <ActivityDetailsSidebar activity={activity}/>
            </Grid>
        </Grid>
  )
}

