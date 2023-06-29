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
    
    public async Task<IEnumerable<string>> GetUserRoles(string userId)
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
            Console.WriteLine($"--> Couldn`t call to GRPC server {e.Message}");
            throw;
        }
    }
}