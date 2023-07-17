import { SessionStatus } from "../SessionStatus";

// For the users sessions
export interface UserSessionsViewModel {
  id: string;
  productTitle: string;
  sessionLogsId: string;
  start: Date;
  expiry: Date;
  status: SessionStatus;
}
