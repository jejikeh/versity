import { LogLevel } from "@microsoft/signalr";

export interface LogData {
    id: string;
    time: Date;
    logLevel: LogLevel; 
}