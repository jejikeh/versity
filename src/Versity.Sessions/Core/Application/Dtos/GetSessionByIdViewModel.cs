using Domain.Models;

namespace Application.Dtos;

public record GetSessionByIdViewModel(
    Guid Id,
    string UserId,
    Product Product,
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
            session.Product,
            session.Logs.Id,
            session.Start,
            session.Expiry,
            session.Status);
    }
}