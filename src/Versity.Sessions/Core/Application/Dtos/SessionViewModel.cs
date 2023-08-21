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
            session.ProductId,
            session.LogsId,
            session.Start,
            session.Expiry,
            session.Status);
    }

    public static IEnumerable<SessionViewModel> MapWithModels(IEnumerable<Session> models)
    {
        var viewModels = new List<SessionViewModel>();
        foreach (var session in models) 
        {
            viewModels.Add(MapWithModel(session));
        }

        return viewModels;
    }
}