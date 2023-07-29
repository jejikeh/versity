import React, { FC, useContext, useEffect } from 'react';
import logo from './logo.svg';
import './App.css';
import { Container, Typography, Box, Stack, Button, Grid, Card, CardActions, CardContent, CardActionArea, CardMedia } from '@mui/material';
import LoginForm from '../components/LoginForm';
import { Context } from '../index';
import { observer } from 'mobx-react-lite';
import { Navigate } from 'react-router-dom';

const Home: FC = () => {
  const {store} = useContext(Context);
  useEffect(() => {
    if (localStorage.getItem('token')) {
      store.checkAuth();
    }
  }, [])

  if (!store.isAuth) {
    return (
        <Navigate to="/login"/>
    );
  }

  return (
    <Container sx={{ pt: 8}}>
        <Stack sx={{pb: 8}}spacing={2}>
            <Typography variant="h1" component="h2" sx={{ typography: { sm: 'h2', xs: 'h3' } }}>
            {store.isAuth ? 'Hello, ' + store.userId : 'Please, enter your credentials'}
            </Typography>
            <Typography variant="body1" sx={{ wordWrap: 'break-word'}}>
            Hello, world! Please, enter your credentials
            </Typography>
        </Stack>
        <Grid container sx={{pl: 2, flexDirection: { xs: "column", md: "row"}}} spacing={2}>
            <Grid xs={6}>
                <Card sx={{ maxWidth: 345 }}>
                    <CardActionArea>
                        <CardMedia
                        component="img"
                        height="240"
                        image="/images/photo_2023-07-14_12-56-24.jpg"
                        alt="green iguana"
                        />
                        <CardContent>
                        <Typography gutterBottom variant="h5" component="div">
                            Manage Sessions
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Lizards are a widespread group of squamate reptiles, with over 6,000
                            species, ranging across all continents except Antarctica
                        </Typography>
                        </CardContent>
                    </CardActionArea>
                </Card>
            </Grid>
            <Grid xs={6}>
                <Card sx={{ maxWidth: 345 }}>
                    <CardActionArea href='/sessions'>
                        <CardMedia
                        component="img"
                        height="240"
                        image="/images/photo_2023-07-14_12-56-24.jpg"
                        alt="green iguana"
                        />
                        <CardContent>
                        <Typography gutterBottom variant="h5" component="div">
                            Sessions
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Lizards are a widespread group of squamate reptiles, with over 6,000
                            species, ranging across all continents except Antarctica
                        </Typography>
                        </CardContent>
                    </CardActionArea>
                </Card>
            </Grid>
        </Grid>
    </Container>
  );
}

export default observer(Home);