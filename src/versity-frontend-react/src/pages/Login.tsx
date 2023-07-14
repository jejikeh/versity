import React, { FC, useContext, useEffect } from 'react';
import logo from './logo.svg';
import './App.css';
import { Container, Typography, Box, Stack } from '@mui/material';
import LoginForm from '../components/LoginForm';
import { Context } from '../index';
import { observer } from 'mobx-react-lite';

const Login: FC = () => {
  const {store} = useContext(Context);
  useEffect(() => {
    if (localStorage.getItem('token')) {
      store.checkAuth();
    }
  }, [])
  return (
    <Container sx={{ justifyContent: 'center', alignItems: 'center', pt: 8}}>
      <Stack sx={{ display: 'grid', justifyContent: 'center', alignItems: 'center', textAlign: 'center' }} spacing={2}>
        <Typography variant="h1" component="h2" sx={{ typography: { sm: 'h2', xs: 'h3' } }}>
          Please, enter your credentials
        </Typography>
        <Typography variant="body1" sx={{ wordWrap: 'break-word'}}>
          Hello, world! Please, enter your credentials
        </Typography>
        <LoginForm/>
      </Stack>
    </Container>
  );
}

export default observer(Login);