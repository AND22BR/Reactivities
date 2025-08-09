import { DeleteOutline, Delete } from "@mui/icons-material";
import { Box, Button } from "@mui/material";

export default function DeleteButton() {
  return (
    <Box sx={{position: 'relative'}}>
        <Button 
            sx={{
                opacity: 0.8,
                transition: 'opacity 0.3s',
                position: 'relative',
                cursor: 'pointer'
            }}>
                <DeleteOutline
                    sx={{
                        fontSize: 32,
                        color: 'white',
                        position: 'relative',
                    }}>
                         <Delete
                    sx={{
                        fontSize: 28,
                        color: 'red'
                    }}>

                    </Delete>
                    </DeleteOutline>
               
        </Button>

    </Box>
  )
}