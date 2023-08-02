namespace Users.Tests.Integrations.Helpers.Mocks;

public interface IGrpcUsersDataServiceMock
{
    public Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    public Task<bool> IsUserExistAsync(string userId);
}