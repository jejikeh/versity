using Domain.Models;

namespace Application.Abstractions;

public interface IEmailConfirmMessageService
{
    public Task SendEmailConfirmMessageAsync(VersityUser user);
}