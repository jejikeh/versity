using Application.RequestHandlers.Auth.Commands.ConfirmEmail;

namespace Users.Tests.Application.RequestHandlers.Auth;

public class ConfirmEmailRequestHandlerTest
{
    //private 
    
    [Fact]
    public void Handle_Should_ReturnFailureResult_WhenUserDoesNotExist()
    {
        //var command = new ConfirmEmailCommand("test", "test");
        //var handler = new ConfirmEmailCommandHandler();
    }
}