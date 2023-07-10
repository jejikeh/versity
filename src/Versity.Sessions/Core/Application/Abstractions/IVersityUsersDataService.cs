namespace Application.Abstractions;

public interface IVersityUsersDataService
{
    public Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    public Task<bool> IsUserExistAsync(string userId);
}