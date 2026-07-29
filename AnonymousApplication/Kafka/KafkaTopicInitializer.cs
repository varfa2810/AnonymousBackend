using AnonymousApplication.DTOs;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Kafka
{
    public sealed class KafkaTopicInitializer
    {
        private readonly KafkaOptions _options;

        public KafkaTopicInitializer(IOptions<KafkaOptions> options)
        {
            _options = options.Value;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _options.BootstrapServers
            };

            using var admin = new AdminClientBuilder(config).Build();

            var metadata = admin.GetMetadata(TimeSpan.FromSeconds(10));

            var existingTopics = metadata.Topics
                                         .Select(x => x.Topic)
                                         .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var topicsToCreate = KafkaTopics.All
                .Where(x => !existingTopics.Contains(x.Name))
                .Select(x => new TopicSpecification
                {
                    Name = x.Name,
                    NumPartitions = x.Partitions,
                    ReplicationFactor = x.ReplicationFactor
                })
                .ToList();

            if (topicsToCreate.Count == 0)
                return;

            await admin.CreateTopicsAsync(topicsToCreate);
        }
    }
}
