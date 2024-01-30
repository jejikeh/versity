using Grpc.Net.Client;
using Presentation;

namespace Users.Tests.Integrations.Helpers.Mocks;

public static class GrpcUsersDataServiceMock
{
    public static async Task<IEnumerable<string>> GetUserRolesAsync(this GrpcChannel channel, string userId)
    {
        var client = new GrpcUsers.GrpcUsersClient(channel);
        var request = new GetUserRolesRequest
        {
            UserId = userId
        };

        var reply = await client.GetUserRolesAsync(request);
            
        return reply.Roles.ToList();
    }

    public static async Task<bool> IsUserExistAsync(this GrpcChannel channel, string userId)
    {
        var client = new GrpcUsers.GrpcUsersClient(channel);
        var request = new GrpcIsUserExistRequest()
        {
            UserId = userId
        };

        var reply = await client.IsUserExistAsync(request);
            
        return reply.Exist;
    }
}