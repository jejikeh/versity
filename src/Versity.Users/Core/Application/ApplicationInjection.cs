using System.Reflection;

namespace Versity.Users.Core.Application;

public static class ApplicationInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        // NOTE: in the future, there may be other services in the application that we can inject here
        serviceCollection.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        return serviceCollection;
    }
}