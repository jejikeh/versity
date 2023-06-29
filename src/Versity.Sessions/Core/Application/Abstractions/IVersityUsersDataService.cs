namespace Application.Abstractions;

public interface IVersityUsersDataClient
{
    public IEnumerable<string> GetUserRoles(string userId);
}