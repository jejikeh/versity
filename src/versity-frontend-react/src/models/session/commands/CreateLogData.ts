import { LogLevel } from "@microsoft/signalr";

export interface CreateLogData {
  sessionLogsId: string;
  time: string;
  logLevel: LogLevel;
  data: string;
}

export class CreateLogDataHelper {
  public static toCreateLogData(
    sessionLogsId: string,
    logLevel: LogLevel,
    data: string
  ): CreateLogData {
    const time = new Date(Date.now());
    console.log(
      `${this.AddZero(time.getFullYear().toString())}-${this.AddZero(
        (time.getMonth() + 1).toString()
      )}-${this.AddZero(time.getDate().toString())}T${this.AddZero(
        time.getHours().toString()
      )}:${this.AddZero(time.getMinutes().toString())}:${this.AddZero(
        time.getSeconds().toString()
      )}`
    );
    return {
      sessionLogsId,
      time: `${this.AddZero(time.getFullYear().toString())}-${this.AddZero(
        (time.getMonth() + 1).toString()
      )}-${this.AddZero(time.getDate().toString())}T${this.AddZero(
        time.getHours().toString()
      )}:${this.AddZero(time.getMinutes().toString())}:${this.AddZero(
        time.getSeconds().toString()
      )}`,
      logLevel,
      data,
    };
  }

  private static AddZero(st: string): string {
    if (st.length === 1) {
      return "0" + st;
    }

    return st;
  }
}
