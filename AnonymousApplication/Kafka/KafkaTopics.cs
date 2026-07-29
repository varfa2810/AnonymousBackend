using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Kafka
{
    public static class KafkaTopics
    {

        public const string MessageCreated = "message.created.v1";

        public static readonly KafkaTopicDefinition[] All =
    {
        new()
        {
            Name = "message.created.v1",
            Partitions = 12,
            ReplicationFactor = 1
        },

    };

    }

    public sealed class KafkaTopicDefinition
    {
        public string Name { get; init; } = string.Empty;

        public int Partitions { get; init; }

        public short ReplicationFactor { get; init; }
    }
}
