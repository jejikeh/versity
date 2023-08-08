namespace Sessions.Tests.Integrations.Helpers.Http;

public static class SessionLogsHttpHelper
{
    public static string GetSessionsLogs(int page) => "/api/sessionlogs/" + page;
    public static string GetSessionLogsById(string guid) => "/api/sessionlogs/" + guid;
}