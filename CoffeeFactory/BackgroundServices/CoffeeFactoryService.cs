using CoffeeFactory.Clients;
using SharedModels.Dtos;
using SharedModels.Enums;
using System.Collections.Concurrent;

namespace CoffeeFactory.BackgroundServices
{
    public class CoffeeFactoryService : BackgroundService
    {
        private readonly TimeSpan _coffeeCreationInterval = TimeSpan.FromSeconds(5);
        private IMessageBrokerClient _messageBrokerClient;
        
        private static int _serialNumber = 0;
        private static ConcurrentStack<CoffeePublishMessageDto> _coffeesToSend = new ConcurrentStack<CoffeePublishMessageDto>();

        public CoffeeFactoryService(IMessageBrokerClient messageBrokerClient)
        {
            _messageBrokerClient = messageBrokerClient ?? throw new ArgumentNullException(nameof(messageBrokerClient));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //force this method to run async right away
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                CreateCoffeeAsync();
                //wait 5 seconds before creating another coffee
                await Task.Delay(_coffeeCreationInterval, stoppingToken);
            }
        }

        private async void CreateCoffeeAsync()
        {
            _serialNumber++;

            var coffeePublishMessageDto = new CoffeePublishMessageDto
            {
                SerialNumber = _serialNumber,
                Name = _serialNumber % 2 == 0 ? CoffeeType.Arabica.ToString() : CoffeeType.Robusta.ToString()
            };

            //put the coffeePublishMessageDto on a threadsafe stack.
            _coffeesToSend.Push(coffeePublishMessageDto);
            //run this method on another thread to catch up with retried messages
            await Task.Run( () => SendMessage());
        }

        private void SendMessage()
        {
            //try and get a coffeePublishMessageDto from the stack.
            //if successful, send a message
            while(_coffeesToSend.TryPop(out var coffeePublishMessageDto))
            {
                if (coffeePublishMessageDto != null)
                {
                    var messageSent = _messageBrokerClient.PublishNewCoffee(coffeePublishMessageDto);
                    // if the message could not be sent, put it back on the stack to retry.
                    if (messageSent == false)
                        _coffeesToSend.Push(coffeePublishMessageDto);
                }
            }
        }
    }
}