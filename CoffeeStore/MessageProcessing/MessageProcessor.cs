using AutoMapper;
using CoffeeStore.Models;
using CoffeeStore.Repositories;
using SharedModels.Dtos;
using System.Text.Json;

namespace CoffeeStore.MessageProcessing
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public MessageProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public void ProcessMessage(string message)
        {
            var coffeePublishMessageDto = JsonSerializer.Deserialize<CoffeePublishMessageDto>(message);
            if(coffeePublishMessageDto != null)
            {
                AddCoffeeToDb(coffeePublishMessageDto);
            }
        }

        private void AddCoffeeToDb(CoffeePublishMessageDto coffeePublishMessageDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICoffeeRepo>();
            try
            {
                var coffee = _mapper.Map<Coffee>(coffeePublishMessageDto);
                repo.CreateCoffee(coffee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Coffee could not be added to DB {ex.Message}");
            }
        }
    }
}
