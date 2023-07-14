namespace Application.Abstractions;

public interface ISessionClient
{
    public Task ConnectToSession(string message);
}