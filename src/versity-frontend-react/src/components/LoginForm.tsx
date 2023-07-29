import React, {FC, useContext, useState} from "react"
import { Container, Typography, Box, TextField, Stack, Button, Link } from '@mui/material';
import { Context } from "..";
import { observer } from "mobx-react-lite";
import { Navigate } from "react-router-dom";

const LoginForm: FC = () => {
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const {store} = useContext(Context);

    if (store.isAuth) {
        return (
            <Navigate to="/"/>
        );
      }

    return (
        <Container sx={{ pt: 8}}>
            <Stack sx={{ textAlign: 'center', pb: 4 }} spacing={2}>
                <Typography variant="h1" component="h2" sx={{ typography: { sm: 'h2', xs: 'h3' } }}>
                Please, enter your credentials
                </Typography>
                <Typography variant="body1" sx={{ wordWrap: 'break-word'}}>
                Hello, world! Please, enter your credentials
                </Typography>
            </Stack>
            <Box sx={{ maxWidth: 500, mx: 'auto'}}>
                <Stack spacing={2}>
                    <Box>
                        <Stack spacing={2}>
                            <TextField 
                                id="outlined-basic" 
                                label="Email" 
                                variant="outlined"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)} 
                            />
                            <TextField 
                                id="outlined-password-input" 
                                label="Password" 
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)} 
                            />
                        </Stack>
                        <Link href="/register" underline="none">Dosen't have an account?</Link>
                    </Box>
                    <Button 
                        variant="outlined" 
                        size="large"
                        onClick={() => store.login(email, password)}
                    >
                            Sign in
                    </Button>
                </Stack>
            </Box>
        </Container>
    )
}

export default observer(LoginForm);