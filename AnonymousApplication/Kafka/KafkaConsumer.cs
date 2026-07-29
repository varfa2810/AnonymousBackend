using AnonymousApplication.DTOs;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AnonymousApplication.Kafka
{
    public sealed class KafkaConsumer : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(IOptions<KafkaOptions> options, ILogger<KafkaConsumer> logger)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                GroupId = options.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            _logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _consumer.Subscribe(new[]
                    {
                        KafkaTopics.MessageCreated
                    });

                    _logger.LogInformation("Kafka Consumer Started.");

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = _consumer.Consume(stoppingToken);

                        if (result == null)
                            continue;

                        var message = JsonSerializer.Deserialize<MessageCreatedEvent>(
                            result.Message.Value);

                        //Here add the logic for consuption..
                        _logger.LogInformation($"Consuption message: {message?.Message}");

                        _consumer.Commit(result);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Consume Error : {ex.Error.Reason}");

                    _consumer.Unsubscribe();

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (KafkaException ex)
                {
                    _logger.LogError($"Kafka Error : {ex.Error.Reason}");

                    _consumer.Unsubscribe();

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
