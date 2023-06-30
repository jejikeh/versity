using Application.Abstractions;
using Grpc.Net.Client;

namespace Presentation.Services;

public class GrpcUsersDataService : IVersityUsersDataService
{
    private readonly IConfiguration _configuration;

    public GrpcUsersDataService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var channel = GrpcChannel.ForAddress(_configuration["GrpcUsers"]);
        var client = new GrpcUsers.GrpcUsersClient(channel);
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
            Console.WriteLine($"--> Could`t call to GRPC server {e.Message}");
            throw;
        }
    }

    public async Task<bool> IsUserExistAsync(string userId)
    {
        var channel = GrpcChannel.ForAddress(_configuration["GrpcUsers"]);
        var client = new GrpcUsers.GrpcUsersClient(channel);
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
            Console.WriteLine($"--> Could`t call to GRPC server {e.Message}");
            throw;
        }
    }
}