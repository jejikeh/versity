using Domain.Models;

namespace Application.Dtos;

public record UserSessionsViewModel(
    Guid Id, 
    Guid ProductId,
    Guid SessionLogsId,
    DateTime Start,
    DateTime Expiry,
    SessionStatus Status)
{
    public static UserSessionsViewModel MapWithModel(Session session)
    {
        return new UserSessionsViewModel(
            session.Id,
            session.ProductId,
            session.LogsId,
            session.Start,
            session.Expiry,
            session.Status);
    }

    public static IEnumerable<UserSessionsViewModel> MapWithModels(IEnumerable<Session> models)
    {
        var viewModels = new List<UserSessionsViewModel>();
        foreach (var session in models) 
        {
            viewModels.Add(MapWithModel(session));
        }

        return viewModels;
    }
}