using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace EventBus
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IModel _channel;

        public RabbitMQEventBus()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName="guest", Password="guest" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "microservices_exchange", type: "direct");
        }

        public void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "microservices_exchange", routingKey: eventName, basicProperties: null, body: body);
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            // Логика подписки при необходимости
        }
    }
}