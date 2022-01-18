using SharedModels.Dtos;

namespace CoffeeFactory.Clients
{
    public interface IMessageBrokerClient
    {
        bool PublishNewCoffee(CoffeePublishMessageDto coffeePublishMessageDto);
    }
}
