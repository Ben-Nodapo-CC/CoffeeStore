using CoffeeStore.MessageProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CoffeeStore.BackgroundServices
{
    public class MessageBrokerSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageProcessor _messageProcessor;
        
        private IConnection _connection;
        private IModel _channel;

        public MessageBrokerSubscriber(IConfiguration configuration, IMessageProcessor messageProcessor)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));

            InitializeMessageBroker();
        }

        private void InitializeMessageBroker()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection("CoffeConnection");
                _channel = _connection.CreateModel();
                _channel.BasicQos(0, 1, false);
                _connection.ConnectionShutdown += MessageBrokerConnectionShutdown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not establish connection to message broker {ex.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, eventargs) =>
            {
                var messageBody = eventargs.Body;
                var message = Encoding.UTF8.GetString(messageBody.ToArray());
                _messageProcessor.ProcessMessage(message);
            };

            _channel.BasicConsume(queue: "CoffeeCreated", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }

        private void MessageBrokerConnectionShutdown(object sender, ShutdownEventArgs e) =>
          Console.WriteLine("The Connection to the Message Broker shut down");
    }
}
