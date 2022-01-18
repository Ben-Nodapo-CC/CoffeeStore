using CoffeeStore.BackgroundServices;
using CoffeeStore.MessageProcessing;
using CoffeeStore.Repositories;

namespace CoffeeStore.Bootstrapping
{
    public static class DIExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICoffeeRepo, CoffeeRepo>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHostedService<MessageBrokerSubscriber>();
            services.AddSingleton<IMessageProcessor, MessageProcessor>();

            return services;
        }
    }
}
