namespace Sessions.IntegrationTests.Helpers.Http;

public static class SessionHttpHelper
{
    public static string GetAllSessions(int page) => "/api/sessions/" + page;
    public static string GetAllProducts(int page) => "/api/sessions/products/" + page;
    public static string GetSessionById(string guid) => "/api/sessions/" + guid;
    public static string GetUserSessionsByUserId(string id, int page) => $"/api/sessions/users/{id}/{page}";
    public static string GetAllProductSessions(string id, int page) => $"/api/sessions/products/{id}/{page}";
    public static string CreateSession() => "api/sessions/";
    public static string DeleteSession(string id) => $"/api/sessions/{id}";
    public static string CloseSession(string id) => $"/api/sessions/{id}/close";
}