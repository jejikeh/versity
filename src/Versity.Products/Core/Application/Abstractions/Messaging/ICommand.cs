using FluentResults;
using MediatR;

namespace Application.Abstractions.Messaging;

public interface ICommand<T> : IRequest<Result<T>>
{
    
}

public interface ICommand : IRequest<Result>
{
    
}