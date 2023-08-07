using Application.Abstractions;
using Grpc.Core;
using Infrastructure;
using MediatR;

namespace Sessions.Tests.Integrations.Helpers;

public class GrpcUsersServiceMock : IVersityUsersDataService
{
    public Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        return Task.FromResult((IEnumerable<string>)new []{ "Admin", "Mebmer" });
    }

    public Task<bool> IsUserExistAsync(string userId)
    {
        return Task.FromResult(true);
    }
}