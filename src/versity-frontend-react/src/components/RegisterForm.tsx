import React, { FC, useContext, useState } from "react"
import { Container, Typography, Box, TextField, Stack, Button, Link } from '@mui/material';
import { Context } from "..";
import { observer } from "mobx-react-lite";

const RegisterForm: FC = () => {
  const [firstName, setFirstName] = useState<string>('');
  const [lastName, setLastName] = useState<string>('');
  const [email, setEmail] = useState<string>('');
  const [phone, setPhone] = useState<string>('');
  const [password, setPassword] = useState<string>('');
  const { store } = useContext(Context);

  return (
    <Container sx={{ justifyContent: 'center', alignItems: 'center', pt: 8 }}>
      <Stack sx={{ display: 'grid', justifyContent: 'center', alignItems: 'center', textAlign: 'center', pb: 4 }} spacing={2}>
        <Typography variant="h1" component="h2" sx={{ typography: { sm: 'h2', xs: 'h3' } }}>
          Please, enter your credentials
        </Typography>
        <Typography variant="body1" sx={{ wordWrap: 'break-word' }}>
          Hello, world! Please, enter your credentials
        </Typography>
      </Stack>
      <Box sx={{ maxWidth: 500, mx: 'auto' }}>
        <Stack spacing={2}>
          <Box>
            <Stack spacing={2}>
              <TextField
                fullWidth
                id="outlined-basic"
                label="First Name"
                variant="outlined"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
              />
              <TextField
                fullWidth
                id="outlined-basic"
                label="Last Name"
                variant="outlined"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
              />
              <TextField
                fullWidth
                id="outlined-basic"
                label="Phone Number"
                variant="outlined"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
              />
              <TextField
                fullWidth
                id="outlined-basic"
                label="Email"
                variant="outlined"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
              <TextField
                fullWidth
                id="outlined-password-input"
                label="Password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </Stack>
            <Link href="/login" underline="none">Already have an account?</Link>
          </Box>
          <Button
            variant="outlined"
            size="large"
            onClick={() => store.registraction(firstName, lastName, email, phone, password)}
          >
            Sign up
          </Button>
        </Stack>
      </Box>
    </Container>
  )
}

export default observer(RegisterForm);
