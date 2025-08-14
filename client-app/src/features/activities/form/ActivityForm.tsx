import { Box, Button, Paper, Typography } from "@mui/material";
import { useEffect } from "react";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useNavigate, useParams } from "react-router";
import { useForm, Resolver } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod';
import { activitySchema, ActivitySchema } from "../../../lib/schemas/activitySchema";
import TextInput from "../../../app/shared/components/TextInput";
import SelectInput from "../../../app/shared/components/SelectInput";
import { categoryOptions } from "./categoryOptions";
import DateTimeInput from "../../../app/shared/components/DateTimeInput";
import LocationInput from "../../../app/shared/components/LocationInput";

export default function ActivityForm() {
  const { reset, handleSubmit, control } = useForm<ActivitySchema>({
    mode: 'onTouched',
    resolver: zodResolver(activitySchema) as Resolver<ActivitySchema, any, ActivitySchema>
  });
  const navigate=useNavigate();
  const {id}=useParams();
  const {updateActivity, createActivity, activity, isLoadingActivity}=useActivities(id);

  useEffect(()=>{
    if(activity) reset({
      ...activity,
      location: {
        city: activity.city,
        venue: activity.venue,
        latitude: activity.latitude,
        longitude: activity.longitude
      }
    });
    [activity, reset]
  })

  const onSubmit=async (data: ActivitySchema)=>{
      const {location,...rest}=data;
      const flattenedData={...rest, ...location}

      try {
        if(activity){
          updateActivity.mutate({...activity, ...flattenedData}, {
            onSuccess: () => navigate(`/activities/${activity.id}`)
          })
        }
        else{
          createActivity.mutate(flattenedData, {
            onSuccess: (id) => navigate(`/activities/${id}`)
          })
        }
      } catch (error) {
        console.log(error);
      }
    // if(activity){
    //   data.id= activity.id;
    //   await updateActivity.mutateAsync(data as unknown as Activity);
    //   navigate(`/activities/${activity.id}`);
    // } else{
    //   createActivity.mutateAsync(data as unknown as Activity, {
    //     onSuccess:(id)=>{
    //       navigate(`/activities/${id}`);
    //     }
    //   });
    // }
  }

  if(isLoadingActivity){
    return(<Typography>Loading...</Typography>)
  }

  return (
    <Paper sx={{borderRadius:3, padding:3}}>
        <Typography variant="h5" gutterBottom color="primary">
            {activity? 'Edit activity' : 'Create activity'}
        </Typography>
        <Box component="form" onSubmit={handleSubmit(onSubmit)} display="flex" flexDirection="column" gap={3}>
            <TextInput<ActivitySchema> label='Title' control={control} name='title'/>
            <TextInput<ActivitySchema> label='Description' control={control} multiline rows={3} name='description'/>
            <Box display='flex' gap={3}>
              <SelectInput<ActivitySchema> items={categoryOptions} label='Category' control={control} name='category'/>
              <DateTimeInput<ActivitySchema> label='Date' control={control} name='date'/>
            </Box>
            <LocationInput<ActivitySchema> label='Enter the location' control={control} name='location'></LocationInput>
            <Box display="flex" justifyContent="end" gap={3}>
                <Button color="inherit">Cancel</Button>
                <Button 
                  type="submit" 
                  color="success" 
                  variant="contained"
                  disabled={updateActivity.isPending || createActivity.isPending}  
                >Submit</Button>
            </Box>
        </Box>
    </Paper>
  )
}

