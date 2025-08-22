import { zodResolver } from "@hookform/resolvers/zod";
import { useAccount } from "../../lib/hooks/useAccount"
import { LoginSchema, loginSchema } from "../../lib/schemas/loginSchema";
import { Paper, Box, Typography, Button } from "@mui/material";
import { useForm } from "react-hook-form";
import { LockOpen } from "@mui/icons-material";
import TextInput from "../../app/shared/components/TextInput";
import { Link, useLocation, useNavigate } from "react-router";
import { useState } from "react";
import { toast } from "react-toastify";

export default function LoginForm() {
    const [notVerified, setNotVerified]=useState(false);
    const { loginUser, resendConfirmationEmail } = useAccount();
    const navigate=useNavigate();
    const location=useLocation();
    const { control, handleSubmit, watch, formState: { isValid, isSubmitting } } = useForm<LoginSchema>({
        mode: 'onTouched',
        resolver: zodResolver(loginSchema)
    });

    const email=watch('email');

    const handleResendEmail=async ()=>{
        try {
            await resendConfirmationEmail.mutateAsync({email});
            setNotVerified(false);    
        } catch (error) {
            console.log(error);
            toast.error('Problem sending email. Please check email address');
        }
    }

    const onSubmit = async (data: LoginSchema) => {
        await loginUser.mutateAsync(data, {
            onSuccess: ()=>{
                navigate(location.state?.from || '/activities');
            },
            onError: error=>{
                if(error.message==='NotAllowed'){
                    setNotVerified(true);
                }
            }
        });
    }
    return (
        <Paper component='form'
            onSubmit={handleSubmit(onSubmit)}
            sx={{
                dispaly: 'flex',
                flexDirection: 'column',
                p: 3,
                gap: 3,
                maxWidth: 'md',
                mx: 'auto',
                borderRadius: 3
            }}>
            <Box display='flex' alignItems='center' justifyContent='center'
                gap={3}
                color='secondary.main'
            >
                <LockOpen fontSize="large" />
                <Typography variant="h4">Sign in</Typography>
            </Box>

            <TextInput label='Email' control={control} name='email' type="email"></TextInput>
            <TextInput label='Password' control={control} name='password' type="password"></TextInput>
            <Button
                type='submit'
                disabled={!isValid || isSubmitting}
                variant="contained"
                size="large">
                Login
            </Button>
            {notVerified ? (
                <Box display='flex' flexDirection='column' justifyContent='center'>
                    <Typography textAlign='center' color='error'>
                        Your email has not been verified. You can click the button to re-send the email.
                    </Typography>
                    <Button
                        disabled={resendConfirmationEmail.isPending}
                        onClick={handleResendEmail}
                    >
                        Re-send email link
                    </Button>
                </Box>
            ):(
                <Typography sx={{textAlign: 'center'}}>
                Don't have an account?
                <Typography component={Link} to='/register' color='primary' sx={{ml: 2}}>
                Sign up
                </Typography>
            </Typography>
            )}
        </Paper>
    )
}