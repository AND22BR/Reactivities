import { useParams } from "react-router"
import { useProfile } from "../../lib/hooks/useProfile";
import { Box, Button, Divider, Typography } from "@mui/material";

export default function PrrofileAbout() {

    const {id}=useParams();
    const {profile}=useProfile();

  return (
    <Box>
        <Box  display='flex' justifyContent='space-between'>
            <Typography></Typography>
            <Button>
                Edit profile
            </Button>
        </Box>
        <Divider></Divider>
        <Box sx={{overlfow: 'auto'}}>
            <Typography variant="body1" sx={{whiteSpace: 'pre-wrap'}}>
                {profile?.bio || 'No description added yet'}
            </Typography>
        </Box>
    </Box>
  )
}