using AnonymousApplication.DTOs;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AnonymousApplication.Kafka
{
    public class KafkaProducerService : IKafkaProducer
    {
        private readonly IProducer<Null, string> _producer;
        private readonly KafkaOptions _options;

        public KafkaProducerService(IOptions<KafkaOptions> options)
        {
            _options = options.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = _options.BootstrapServers,

                Acks = Acks.All,

                EnableIdempotence = true,

                MessageSendMaxRetries = 3
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();

        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(
                topic,
                new Message<Null, string>
                {
                    Value = json
                });
        }

        public async Task PublishJsonAsync(string topic, string json)
        {
            try
            {
                await _producer.ProduceAsync(topic, new Message<Null, string>
                {
                    Value = json
                });
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"Code      : {ex.Error.Code}");
                Console.WriteLine($"Reason    : {ex.Error.Reason}");
                Console.WriteLine($"IsFatal   : {ex.Error.IsFatal}");
                Console.WriteLine($"Topic     : {topic}");

                throw;
            }
        }
    }
}
