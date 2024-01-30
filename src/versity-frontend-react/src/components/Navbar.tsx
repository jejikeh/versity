import React, {FC, useContext, useEffect, useState} from "react"
import { AppBar, Toolbar, IconButton, Typography, Stack, Button, Link } from "@mui/material"
import { Context } from "..";
import { Navigate } from 'react-router-dom';

const NavBar: FC = () => {
  const {store} = useContext(Context);
  useEffect(() => {
    if (localStorage.getItem('token')) {
      store.checkAuth();
    }
  }, [])

  return (
    <AppBar position='static' sx={{ bgcolor: "black" }}>
        <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                VERSITY
            </Typography>
        <Stack direction={"row"} spacing={2}>
            <Button color="inherit">
                <Link href="/" underline="none" sx={{color: "white"}}>Home</Link>
            </Button>
            <Button color="inherit" onClick={() => store.logout()}>
                Sign out
            </Button>
        </Stack>
        </Toolbar>
    </AppBar>
  );
}

export default NavBar;