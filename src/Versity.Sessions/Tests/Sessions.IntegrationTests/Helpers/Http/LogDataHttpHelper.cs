namespace Sessions.IntegrationTests.Helpers.Http;

public static class LogDataHttpHelper
{
    public static string GetAllLogsData(int page) => "/api/logdata/" + page;
    public static string GetLogDataById(string id) => "/api/logdata/" + id;
    public static string CreateLogData() => "api/logdata/";
    public static string CreateLogsData(string id) => "api/logdata/" + id;
}