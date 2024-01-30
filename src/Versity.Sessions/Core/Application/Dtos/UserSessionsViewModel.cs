using Domain.Models;

namespace Application.Dtos;

public record UserSessionsViewModel(
    Guid Id, 
    string ProductTitle,
    Guid SessionLogsId,
    DateTime Start,
    DateTime Expiry,
    SessionStatus Status)
{
    public static UserSessionsViewModel MapWithModel(Session session)
    {
        return new UserSessionsViewModel(
            session.Id,
            session.Product.Title,
            session.Logs.Id,
            session.Start,
            session.Expiry,
            session.Status);
    }

    public static IEnumerable<UserSessionsViewModel> MapWithModels(List<Session> models)
    {
        var viewModels = new List<UserSessionsViewModel>();
        foreach (var session in models) 
        {
            viewModels.Add(MapWithModel(session));
        }

        return viewModels;
    }
}