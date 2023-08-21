namespace Infrastructure.Exceptions;

public class UserSecretsInvalidException : Exception
{
    public UserSecretsInvalidException(string scriptName) 
        : base($"Please, make sure what you setup your UserSecrets storage. " +
               $"You can use the '{scriptName}' script from the Makefile at root directory or setup " +
               $"appsettings.Environment.json")
    {
    }
}