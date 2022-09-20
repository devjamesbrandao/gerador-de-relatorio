using RabbitMQ.Client;
using Relatorio.Core.Interfaces;

namespace DevFreela.infrastructure.MessageBus
{
    public class MessageBusPublishService : IMessageBusPublishService
    {
        private readonly ConnectionFactory _factory;

        public MessageBusPublishService()
        {
            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672
            };

            _factory.UserName = "guest";

            _factory.Password = "guest";
        }

        public void Publish(string queue, byte[] message)
        {
            using (var connection = _factory.CreateConnection()) 
            { 
                using(var chanel = connection.CreateModel())
                {
                    chanel.QueueDeclare(
                        queue: queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    chanel.BasicPublish(
                        exchange: "",
                        routingKey: queue,
                        basicProperties: null,
                        body: message
                    );
                }
            }
        }
    }
}