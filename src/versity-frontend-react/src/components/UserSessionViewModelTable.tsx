import { FC, useContext, useEffect, useState } from "react";
import { Button } from "@mui/material";
import { Context } from "..";
import { IUserSessionViewModelTableProps } from "../models/components/IUserSessionViewModelTableProps";
import TableCell from "@mui/material/TableCell";
import TableRow from "@mui/material/TableRow";
import { SessionStatus } from "../models/session/SessionStatus";
import { DateTimeField } from "@mui/x-date-pickers/DateTimeField";
import dayjs from "dayjs";

const UserSessionsViewModelTable = ({
  model,
}: IUserSessionViewModelTableProps) => {
  return (
    <TableRow
      key={model?.id}
      sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
    >
      <TableCell component="th" scope="row" sx={{ maxWidth: 100 }}>
        {model?.id}
      </TableCell>
      <TableCell align="right">{model?.productTitle}</TableCell>
      <TableCell align="right">
        <DateTimeField defaultValue={dayjs(model?.start)} />
      </TableCell>
      <TableCell align="right">
        <DateTimeField defaultValue={dayjs(model?.expiry)} />
      </TableCell>
      {model?.status == 0 ? (
        <TableCell align="right">
          <Button
            variant="outlined"
            size="large"
            href={"sessions/" + model?.sessionLogsId}
          >
            Connect
          </Button>
        </TableCell>
      ) : (
        <TableCell align="right">
          <Button disabled variant="outlined" size="large">
            {SessionStatus[model ? model.status : 3]}
          </Button>
        </TableCell>
      )}
    </TableRow>
  );
};

export default UserSessionsViewModelTable;
