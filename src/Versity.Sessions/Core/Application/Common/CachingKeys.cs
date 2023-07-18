using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Application.Common;

public static class CachingKeys
{
    public const string SessionLogs = "session-logs";
    public static string SessionById(Guid id) => "sessions-" + id;
}