namespace Application.Abstractions;

public interface IVersityUsersDataService
{
    public Task<IEnumerable<string>> GetUserRoles(string userId);
}