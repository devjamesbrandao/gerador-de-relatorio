using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Relatorio.Core.Interfaces;
using Relatorio.Data.Context;
using Relatorio.Data.Hubs;

namespace Relatorio.Data.MessageBus.Consumer
{
    public class RelatorioConsumer : BackgroundService
    {
        private const string QUEUE = "Relatorios";
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<RelatorioHub> _hub;

        public RelatorioConsumer(IServiceProvider serviceProvider, IHubContext<RelatorioHub> hub)
        {
            _serviceProvider = serviceProvider;

            _hub = hub;

            var factory = new ConnectionFactory
            {
                 HostName = "rabbitmq", 
                 Port = 5672
            };

            factory.UserName = "guest";

            factory.Password = "guest";

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QUEUE,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var byteArray = eventArgs.Body.ToArray();
                
                var relatorioString = Encoding.UTF8.GetString(byteArray);

                var relatorio = JsonSerializer.Deserialize<Relatorio.Core.Models.Relatorio>(relatorioString);

                _channel.BasicAck(eventArgs.DeliveryTag, false);

                ProcessarRelatorio(relatorio).GetAwaiter().GetResult();
            };

            _channel.BasicConsume(QUEUE, false, consumer);

            return Task.CompletedTask;
        }

        public async Task ProcessarRelatorio(Relatorio.Core.Models.Relatorio relatorio)
        {
            await Task.Delay(relatorio.Duracao * 1000);   

            using(var scope = _serviceProvider.CreateScope())
            {
                var repositorio = scope.ServiceProvider.GetRequiredService<IRelatorioRepository>();

                await repositorio.AtualizarStatusRelatorio(relatorio.Id);
            }

            await _hub.Clients.All.SendAsync("ReceiveMessage", relatorio.Id);
        }
    }
}
