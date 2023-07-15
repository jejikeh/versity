import signalR, { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr"

export default class SessionConnectionService {
    static async connect(user: string, room: string) : Promise<HubConnection> {
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

        return connection;
    }
}