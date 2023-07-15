import React, { FC, useContext, useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';
import { Container, Typography, Box, Stack, Button, Grid, Card, CardActions, CardContent, CardActionArea, CardMedia } from '@mui/material';
import LoginForm from '../components/LoginForm';
import { Context } from '../index';
import { observer } from 'mobx-react-lite';
import { Navigate } from 'react-router-dom';
import SessionService from '../services/SessionService';
import { UserSessionsViewModel } from '../models/session/queries/UserSessionsViewModel';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import { SessionStatus } from '../models/session/SessionStatus';
import { DateTimeField } from '@mui/x-date-pickers/DateTimeField';
import dayjs, { Dayjs } from 'dayjs';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import SessionConnectionService from '../services/SessionConnectionService';

const Sessions: FC = () => {
  function createData(
    ID: string,
    Product: string,
    Start: string,
    Expiried: string,
    Status: string,
  ) {
    return { ID, Product, Start, Expiried, Status };
  }

  const {store} = useContext(Context);
  const [userSessions, setUserSessions] = useState<{
    ID: string;
    Product: string;
    Start: string;
    Expiried: string;
    Status: string;
  }[]>([]);

  useEffect(() => {
    const fetchUserSessions = async () => {
      if (!store.userId) {
        return;
      }
      const response = await SessionService.getUserSessions(store.userId, 1);
      var data : {
        ID: string;
        Product: string;
        Start: string;
        Expiried: string;
        Status: string;
      }[] = [];
      response.data.forEach(session => {
        data.push(createData(session.id, session.productTitle, session.start.toString(), session.expiry.toString(), SessionStatus[session.status]));
      })
      setUserSessions(data);
    };

    fetchUserSessions();
  }, [store.userId]);

  // TODO: remove
  const [connection, setConnection] = useState<HubConnection>();

    const fetchConnection = async () => {
      try {
        var user = "2"
        var room = "3"
        const connection = new HubConnectionBuilder()
            .withUrl("https://localhost:8001/hub", {
                skipNegotiation: true,
                transport: 1
            })
            .configureLogging(LogLevel.Information)
            .build();

        connection.on("receivemessage", (user: string, room: string) => {
            console.log("message:" + user + " " + room);
        });

        await connection.start();
        await connection.invoke("JoinToSession", {user, room});
        setConnection(connection);
      } catch (error) {
        console.log(error);
      }
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
            <TableContainer component={Paper}>
      <Table sx={{ minWidth: 650 }} aria-label="simple table">
        <TableHead>
          <TableRow>
            <TableCell>ID</TableCell>
            <TableCell align="right">Product</TableCell>
            <TableCell align="right">Start</TableCell>
            <TableCell align="right">Expired</TableCell>
            <TableCell align="right">Status</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {userSessions.map((row) => (
            <TableRow
              key={row.ID}
              sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
            >
              <TableCell component="th" scope="row" sx={{maxWidth: 100}}>
                {row.ID}
              </TableCell>
              <TableCell align="right">{row.Product}</TableCell>
              <TableCell align="right">
                <DateTimeField defaultValue={dayjs(row.Start)}/>
              </TableCell>
              <TableCell align="right">
                <DateTimeField defaultValue={dayjs(row.Expiried)}/>
              </TableCell>
              {row.Status == "Open" ? (
                <TableCell align="right">
                  <Button
                    variant="outlined" 
                    size="large"
                    onClick={() => {
                      fetchConnection();
                    }}
                  >
                    Connect
                  </Button>
                </TableCell>
              ) : (<TableCell align="right">
                <Button 
                  disabled
                  variant="outlined" 
                  size="large"
            >
              {row.Status}
            </Button>
            </TableCell>)}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
        </Stack>
        <Button 
            variant="outlined" 
            size="large"
            onClick={() => store}
        >  
        </Button>
    </Container>
  );
}

export default observer(Sessions);