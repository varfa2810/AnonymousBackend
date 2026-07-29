using AnonymousApplication.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnonymousApplication.Kafka
{
    public sealed class OutboxPublisher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDbConnectionFactory _connectionfactory;
        private readonly ILogger<OutboxPublisher> _logger;

        public OutboxPublisher(IServiceScopeFactory scopeFactory, IDbConnectionFactory connectionfactory, ILogger<OutboxPublisher> logger)
        {
            _scopeFactory = scopeFactory;
            _connectionfactory = connectionfactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var connection = _connectionfactory.CreateConnection();

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _scopeFactory.CreateScope();

                    var repository = scope.ServiceProvider.GetRequiredService<IOutbox>();

                    var producer = scope.ServiceProvider.GetRequiredService<IKafkaProducer>();

                    var pendingMessages = await repository.GetPendingMessages(connection);

                    foreach (var message in pendingMessages)
                    {
                        try
                        {
                            await producer.PublishJsonAsync(message.EventType, message.Payload);

                            await repository.MarkAsProcessed(connection, message.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);

                            await repository.IncreaseRetryCount(connection, message.Id);
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Outbox publisher is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Outbox publisher stopped unexpectedly.");
            }
        }
    }
}
