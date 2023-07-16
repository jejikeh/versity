import React, { FC, useContext, useEffect, useState } from "react";
import logo from "./logo.svg";
import "./App.css";
import {
  Container,
  Typography,
  Box,
  Stack,
  Button,
  Grid,
  Card,
  CardActions,
  CardContent,
  CardActionArea,
  CardMedia,
  Pagination,
} from "@mui/material";
import LoginForm from "../components/LoginForm";
import { Context } from "../index";
import { observer } from "mobx-react-lite";
import { Navigate } from "react-router-dom";
import SessionService from "../services/SessionService";
import { UserSessionsViewModel } from "../models/session/queries/UserSessionsViewModel";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import { SessionStatus } from "../models/session/SessionStatus";
import { DateTimeField } from "@mui/x-date-pickers/DateTimeField";
import dayjs, { Dayjs } from "dayjs";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
  IHttpConnectionOptions,
} from "@microsoft/signalr";
import SessionConnectionService from "../services/SessionConnectionService";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import Alert from "@mui/material/Alert";
import CircularProgress from "@mui/material/CircularProgress";
import UserSessionsViewModelTable from "../components/UserSessionViewModelTable";

const Sessions: FC = () => {
  const { store } = useContext(Context);
  const [userSessions, setUserSessions] = useState<UserSessionsViewModel[]>([]);

  const [page, setPage] = useState(1);

  useEffect(() => {
    const fetchUserSessions = async () => {
      if (!store.userId) {
        return;
      }
      const response = await SessionService.getUserSessions(store.userId, page);
      setUserSessions(response.data);
    };

    fetchUserSessions();
  }, [store.userId, page]);

  const [openCreateNotificationState, setCreateNotificationOpen] =
    useState(false);
  const [openCloseNotificationState, setCloseNotificationOpen] =
    useState(false);
  const [newOpenSession, setNewOpenSession] = useState<
    UserSessionsViewModel | undefined
  >();

  const handleCreateNofification = (
    state: boolean,
    viewModel: UserSessionsViewModel | undefined
  ) => {
    setCreateNotificationOpen(state);
    if (viewModel) {
      setNewOpenSession(viewModel);
    }

    if (!state) {
      setPage(1);
    }
  };

  const handleCloseNofification = (
    state: boolean,
    viewModel: UserSessionsViewModel | undefined
  ) => {
    setCloseNotificationOpen(state);
    if (viewModel) {
      setNewOpenSession(viewModel);
    }

    if (!state) {
      setPage(1);
    }
  };

  // TODO: remove
  const [connection, setConnection] = useState<HubConnection>();

  useEffect(() => {
    if (!store.userId) {
      return;
    }

    const fetchConnection = async () => {
      if (!connection || connection.state !== "Connected") {
        setConnection(await SessionConnectionService.connect(store.token));
      }
    };

    connection?.on("creatednewsession", (message: UserSessionsViewModel) => {
      handleCreateNofification(true, message);
    });

    connection?.on("closedsession", (message: UserSessionsViewModel) => {
      handleCloseNofification(true, message);
    });

    fetchConnection();
  }, [store.userId, connection, openCreateNotificationState]);

  const fetchConnection = async () => {
    connection?.stop();
    if (!connection || connection.state !== "Connected") {
      setConnection(await SessionConnectionService.connect(store.token));
    }
  };

  return (
    <Container sx={{ pt: 8 }}>
      <Dialog
        open={openCreateNotificationState}
        onClose={() => handleCreateNofification(false, undefined)}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">
          {"New session was created for your account!"}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            You can view details of the session here.
            <br></br>Click Connect button, if you want to connect to the new
            session, otherwise click Cancel button to refresh the current page.
          </DialogContentText>
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
                <UserSessionsViewModelTable model={newOpenSession} />
              </TableBody>
            </Table>
          </TableContainer>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => handleCreateNofification(false, undefined)}
            autoFocus
          >
            Ok
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog
        open={openCloseNotificationState}
        onClose={() => handleCloseNofification(false, undefined)}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">
          {"Your session was closed or expired!"}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            You can view details of the session here.
          </DialogContentText>
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
                <UserSessionsViewModelTable model={newOpenSession} />
              </TableBody>
            </Table>
          </TableContainer>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => handleCloseNofification(false, undefined)}
            autoFocus
          >
            Ok
          </Button>
        </DialogActions>
      </Dialog>
      <Stack sx={{ pb: 8 }} spacing={2}>
        <Typography
          variant="h1"
          component="h2"
          sx={{ typography: { sm: "h2", xs: "h3" } }}
        >
          {store.isAuth
            ? "Your sessions, " + store.userId
            : "Please, enter your credentials"}
        </Typography>
        <Typography variant="body1" sx={{ wordWrap: "break-word" }}>
          Hello, world! Please, enter your credentials
        </Typography>
        {connection && connection.state === "Connected" ? (
          <Alert severity="success">The connection is established</Alert>
        ) : (
          <div>
            <Alert severity="error">
              Error, connection not established! Try refresh the page
            </Alert>
            <Button
              variant="outlined"
              onClick={() => {
                fetchConnection();
              }}
            >
              Try again!
            </Button>
            <CircularProgress sx={{ mt: 2 }} />
          </div>
        )}
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
              {userSessions.map((session) => (
                <UserSessionsViewModelTable model={session} />
              ))}
            </TableBody>
          </Table>
        </TableContainer>
        <Pagination
          count={10}
          onChange={(event, page) => {
            setPage(page);
          }}
        />
      </Stack>
    </Container>
  );
};

export default observer(Sessions);
