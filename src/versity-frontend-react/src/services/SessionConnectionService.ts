import signalR, { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr"

export default class SessionConnectionService {
  static async connect(token: string): Promise<HubConnection | undefined> {
    try {
      const connection = new HubConnectionBuilder()
        .withUrl("http://localhost:9080/sessions-hub", {
          skipNegotiation: true,
          transport: 1,
          accessTokenFactory: () => token
        })
        .configureLogging(LogLevel.Information)
        .build();

      connection.on("NewSession", (message: string) => {
        console.log("New Session: " + message);
      });

      await connection.start();
      return connection;
    }
    catch (error) {
      console.log(error);
    }

    return undefined;
  }
}
