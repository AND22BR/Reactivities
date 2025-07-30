import { observer } from 'mobx-react-lite';
import { useStore } from '../../lib/hooks/useStore';
import { Box, Button, ButtonGroup, List, ListItem, ListItemText, Paper, Typography } from '@mui/material';

export default observer(function Counter() {
const {counterStore} = useStore();  

  return (
    <Box display='flex' justifyContent='space-between'>
        <Box sx={{width:'60%'}}>
             <Typography variant='h4' gutterBottom>
            {counterStore.title}
        </Typography>
        <Typography variant='h6' gutterBottom>
            The count is: {counterStore.count}
        </Typography>
        <ButtonGroup sx={{mt:3}}>
            <Button variant='contained' color='error' onClick={()=>counterStore.decrement(1)}>
                Decrement
            </Button>
            <Button variant='contained' color='success' onClick={()=>counterStore.increment(1)}>
                Increment
            </Button>
            <Button variant='contained' color='primary' onClick={()=>counterStore.increment(5)}>
                Increment by 5
            </Button>
        </ButtonGroup>
        </Box>
        <Paper sx={{width:'40%', p:4}}>
            <Typography variant="h5">Counter events ({counterStore.eventCount})</Typography>
            <List>
                {counterStore.events.map((event, index) => (
                    <ListItemText key={index}>{event}</ListItemText>
                        ))}
            </List>
        </Paper>
    </Box>
  )
});