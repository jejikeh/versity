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
import { Navigate, useParams } from "react-router-dom";
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
  Subject,
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

const Session: FC = () => {
  const urlParams = useParams();
  const { store } = useContext(Context);
  const [connection, setConnection] = useState<HubConnection>();

  const setupStreaming = (): Subject<unknown> => {
    const subject = new Subject();
    connection?.send("UploadStream", subject);
    subject?.next("Connected to the hub");
    return subject;
  };

  useEffect(() => {
    if (!store.userId) {
      return;
    }

    const fetchConnection = async () => {
      if (!connection || connection.state !== "Connected") {
        setConnection(await SessionConnectionService.connect(store.token));
      }
    };

    fetchConnection();
  }, [store.userId, connection]);

  const stream = setupStreaming();

  const fetchConnection = async () => {
    connection?.stop();
    if (!connection || connection.state !== "Connected") {
      setConnection(await SessionConnectionService.connect(store.token));
    }
  };

  return (
    <Container sx={{ pt: 8 }}>
      <Stack sx={{ pb: 8 }} spacing={2}>
        <Typography
          variant="h1"
          component="h2"
          sx={{ typography: { sm: "h2", xs: "h3" } }}
        >
          {store.isAuth
            ? "You in session " + urlParams.id
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
        <Stack direction={"row"} spacing={2}>
          <Button color="error" onClick={() => stream?.next("Red Button")}>
            Red Button
          </Button>
          <Button color="success" onClick={() => stream?.next("Green Button")}>
            Green Button
          </Button>
          <Button onClick={() => stream?.next("Blue Button")}>
            Blue Button
          </Button>
        </Stack>
      </Stack>
    </Container>
  );
};

export default observer(Session);
