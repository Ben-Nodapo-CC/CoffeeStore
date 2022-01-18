using CoffeeFactory.BackgroundServices;
using CoffeeFactory.Clients;

namespace CoffeeFactory.Bootstrapping
{
    public static class DIExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBrokerClient, MessageBrokerClient>();
            services.AddHostedService<CoffeeFactoryService>();

            return services;
        }
    }
}
