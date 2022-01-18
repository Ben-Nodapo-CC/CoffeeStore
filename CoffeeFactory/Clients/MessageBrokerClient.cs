using SharedModels.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CoffeeFactory.Clients
{
    public class MessageBrokerClient : IMessageBrokerClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IBasicProperties _channelProperties;

        public MessageBrokerClient(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),
                AutomaticRecoveryEnabled = true
            };

            try
            {
                _connection = factory.CreateConnection("CoffeConnection");
                _channel = _connection.CreateModel();
                _channelProperties = _channel.CreateBasicProperties();
                _channelProperties.DeliveryMode = 2; //1 = non-persistent, 2 = persistent
                _channel.ExchangeDeclare(exchange: "CoffeCreatedNotification", type: ExchangeType.Fanout, durable: true, autoDelete: false);
                _channel.QueueDeclare(queue: "CoffeeCreated", durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(queue: "CoffeeCreated", exchange: "CoffeCreatedNotification", routingKey: "coffee");
                _connection.ConnectionShutdown += MessageBrokerConnectionShutdown;

                Console.WriteLine("Connection to Message Broker established");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection to Message Broker failed: {ex.Message}");
            }
        }

        public bool PublishNewCoffee(CoffeePublishMessageDto coffeePublishMessageDto)
        {
            var message = JsonSerializer.Serialize(coffeePublishMessageDto);

            if (_connection.IsOpen)
            {
                SendMessage(message);
                return true;
            }

            Console.WriteLine("Was unable to send message");
            return false;
        }

        private void SendMessage(string message)
        {
            var messageBody = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "CoffeCreatedNotification", routingKey: "coffee", basicProperties: _channelProperties, body: messageBody);
            Console.WriteLine("message has been sent to message broker");
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void MessageBrokerConnectionShutdown(object sender, ShutdownEventArgs e) =>
            Console.WriteLine("The Connection to the Message Broker shut down");
    }
}
