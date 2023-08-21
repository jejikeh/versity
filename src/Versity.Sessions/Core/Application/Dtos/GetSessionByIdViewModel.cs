using Domain.Models;

namespace Application.Dtos;

public record GetSessionByIdViewModel(
    Guid Id,
    string UserId,
    Guid ProductId,
    Guid LogsId,
    DateTime Start,
    DateTime Expiry,
    SessionStatus Status)
{
    public static GetSessionByIdViewModel MapWithModel(Session session)
    {
        return new GetSessionByIdViewModel(
            session.Id,
            session.UserId,
            session.ProductId,
            session.LogsId,
            session.Start,
            session.Expiry,
            session.Status);
    }
}