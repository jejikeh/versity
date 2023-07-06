using Application.Abstractions;
using Grpc.Net.Client;

namespace Presentation.Services;

public class GrpcUsersDataService : IVersityUsersDataService
{
    private readonly GrpcChannel _channel;
    private readonly ILogger<GrpcUsersDataService> _logger;

    public GrpcUsersDataService(IConfiguration configuration, ILogger<GrpcUsersDataService> logger)
    {
        _logger = logger;
        _channel = GrpcChannel.ForAddress(configuration["GrpcUsers"]);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var client = new GrpcUsers.GrpcUsersClient(_channel);
        var request = new GetUserRolesRequest
        {
            UserId = userId
        };

        try
        {
            var reply = await client.GetUserRolesAsync(request);
            
            return reply.Roles.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"--> Could`t call to GRPC server {e.Message}");
            throw;
        }
    }

    public async Task<bool> IsUserExistAsync(string userId)
    {
        var client = new GrpcUsers.GrpcUsersClient(_channel);
        var request = new GrpcIsUserExistRequest()
        {
            UserId = userId
        };

        try
        {
            var reply = await client.IsUserExistAsync(request);
            
            return reply.Exist;
        }
        catch (Exception e)
        {
            _logger.LogError($"--> Could`t call to GRPC server {e.Message}");
            throw;
        }
    }
}