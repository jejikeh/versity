using Grpc.Net.Client;
using Presentation;

namespace Users.Tests.Integrations.Helpers.Mocks;

public class GrpcUsersDataServiceMock : IGrpcUsersDataServiceMock
{
    private readonly GrpcChannel _channel;

    public GrpcUsersDataServiceMock(GrpcChannel grpcChannel)
    {
        _channel = grpcChannel;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var client = new GrpcUsers.GrpcUsersClient(_channel);
        var request = new GetUserRolesRequest
        {
            UserId = userId
        };

        var reply = await client.GetUserRolesAsync(request);
            
        return reply.Roles.ToList();
    }

    public async Task<bool> IsUserExistAsync(string userId)
    {
        var client = new GrpcUsers.GrpcUsersClient(_channel);
        var request = new GrpcIsUserExistRequest()
        {
            UserId = userId
        };

        var reply = await client.IsUserExistAsync(request);
            
        return reply.Exist;
    }
}