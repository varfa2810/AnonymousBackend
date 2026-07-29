using AnonymousApplication.DTOs;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
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

        public KafkaConsumer(IOptions<KafkaOptions> options)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                GroupId = options.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            //Add more topics here...
            _consumer.Subscribe( new[] 
            {
                KafkaTopics.MessageCreated
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result == null)
                        continue;

                    var message = JsonSerializer.Deserialize<MessageCreatedEvent>(result.Message.Value);

                    //Instead of this add additional logic to perform
                    Console.WriteLine($"Message : {message?.Message}");

                    _consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    throw;
                }

                await Task.CompletedTask;
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
