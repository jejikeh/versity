using Domain.Models;
using Domain.Models.SessionLogging;

namespace Application.Dtos;

public record SessionViewModel(
    Guid Id, 
    string UserId, 
    Guid ProductId, 
    Guid LogsId,
    DateTime Start,
    DateTime Expiry,
    SessionStatus Status)
{
    public static SessionViewModel MapWithModel(Session session)
    {
        return new SessionViewModel(
            session.Id,
            session.UserId,
            session.Product.Id,
            session.Logs.Id,
            session.Start,
            session.Expiry,
            session.Status);
    }

    public static IEnumerable<SessionViewModel> MapWithModels(List<Session> models)
    {
        var viewModels = new List<SessionViewModel>();
        foreach (var session in models) 
        {
            viewModels.Add(MapWithModel(session));
        }

        return viewModels;
    }
}