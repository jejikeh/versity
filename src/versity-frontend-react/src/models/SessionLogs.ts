import { LogData } from "./LogData";

export interface SessionLogs {
    id: string;
    sessionId: string;
    logs: LogData[];
}